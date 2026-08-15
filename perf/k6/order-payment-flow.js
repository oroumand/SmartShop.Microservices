import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5217';

export const options = {
  stages: [
    { duration: '10s', target: 1 },
    { duration: '20s', target: 3 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    http_req_failed: ['rate<0.05'],
    http_req_duration: ['p(95)<1000'],
  },
};

const jsonHeaders = {
  headers: {
    'Content-Type': 'application/json',
  },
};

function parseJson(response) {
  try {
    return JSON.parse(response.body);
  } catch (_) {
    return null;
  }
}

function property(value, name) {
  if (!value || typeof value !== 'object') {
    return undefined;
  }

  return value[name] ?? value[name.charAt(0).toUpperCase() + name.slice(1)];
}

function asArray(value) {
  if (Array.isArray(value)) {
    return value;
  }

  return property(value, 'items') || property(value, 'data') || [];
}

export default function () {
  const productsResponse = http.get(`${BASE_URL}/api/catalog/products`);
  const products = asArray(parseJson(productsResponse));
  const firstProduct = products[0];
  const productId = property(firstProduct, 'id');

  check(productsResponse, {
    'products request returns 200': (response) => response.status === 200,
    'products returned': () => products.length > 0,
    'first product has id': () => Boolean(productId),
  });

  if (!productId) {
    sleep(1);
    return;
  }

  const customerEmail = `k6-user-${__VU}-${__ITER}@example.com`;
  const orderPayload = JSON.stringify({
    customerId: '11111111-1111-1111-1111-111111111111',
    customerName: `k6 user ${__VU}`,
    customerEmail,
    items: [
      {
        productId,
        quantity: 1,
      },
    ],
  });

  const createOrderResponse = http.post(`${BASE_URL}/api/orders`, orderPayload, jsonHeaders);
  const createdOrder = parseJson(createOrderResponse);
  const orderId = property(createdOrder, 'id');

  check(createOrderResponse, {
    'order creation returns 200 or 201': (response) => response.status === 200 || response.status === 201,
    'created order has id': () => Boolean(orderId),
  });

  if (!orderId) {
    sleep(1);
    return;
  }

  sleep(1);

  const paymentPayload = JSON.stringify({
    orderId,
    method: 'Card',
  });

  const createPaymentResponse = http.post(`${BASE_URL}/api/payments`, paymentPayload, jsonHeaders);

  check(createPaymentResponse, {
    'payment creation returns 200 or 201': (response) => response.status === 200 || response.status === 201,
  });

  sleep(1);

  const finalOrderResponse = http.get(`${BASE_URL}/api/orders/${orderId}`);
  const finalOrder = parseJson(finalOrderResponse);
  const finalStatus = property(finalOrder, 'status');

  check(finalOrderResponse, {
    'final order request returns 200': (response) => response.status === 200,
    'final order is paid when status is present': () => !finalStatus || String(finalStatus).toLowerCase() === 'paid',
  });

  sleep(1);
}

import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5217';

export const options = {
  vus: 1,
  iterations: 1,
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<500'],
  },
};

export default function () {
  const health = http.get(`${BASE_URL}/health`);
  check(health, {
    'health returns 200': (response) => response.status === 200,
  });
  sleep(1);

  const products = http.get(`${BASE_URL}/api/catalog/products`);
  check(products, {
    'catalog products returns 200': (response) => response.status === 200,
  });
  sleep(1);

  const orders = http.get(`${BASE_URL}/api/orders`);
  check(orders, {
    'orders returns 200': (response) => response.status === 200,
  });
  sleep(1);

  const payments = http.get(`${BASE_URL}/api/payments`);
  check(payments, {
    'payments returns 200': (response) => response.status === 200,
  });
}

import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:8088';

export const options = {
  stages: [
    { duration: '10s', target: 2 },
    { duration: '20s', target: 5 },
    { duration: '10s', target: 0 },
  ],
  thresholds: {
    http_req_failed: ['rate<0.01'],
    http_req_duration: ['p(95)<750'],
  },
};

function checkReadResponse(response, name) {
  check(response, {
    [`${name} returns 200`]: (r) => r.status === 200,
    [`${name} has a response body`]: (r) => Boolean(r.body) && r.body.length > 0,
  });
}

export default function () {
  const products = http.get(`${BASE_URL}/api/catalog/products`);
  checkReadResponse(products, 'catalog products');
  sleep(1);

  const laptopSearch = http.get(`${BASE_URL}/api/catalog/products/search?query=laptop`);
  checkReadResponse(laptopSearch, 'catalog search laptop');
  sleep(1);

  const phoneSearch = http.get(`${BASE_URL}/api/catalog/products/search?query=phone`);
  checkReadResponse(phoneSearch, 'catalog search phone');
  sleep(1);
}

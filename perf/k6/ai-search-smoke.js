import http from 'k6/http';
import { check, sleep } from 'k6';

const BASE_URL = __ENV.BASE_URL || 'http://localhost:5217';

export const options = {
  vus: 1,
  iterations: 1,
  thresholds: {
    http_req_failed: ['rate<0.10'],
    http_req_duration: ['p(95)<3000'],
  },
};

export default function () {
  // This script is for a later workshop step.
  // It requires Qdrant to be running and OpenAI configuration to be valid.
  // Until those dependencies are configured, these checks are expected to fail.
  const reindexResponse = http.post(`${BASE_URL}/api/ai-search/reindex`);
  check(reindexResponse, {
    'ai search reindex returns 200': (response) => response.status === 200,
  });

  sleep(1);

  const searchResponse = http.get(`${BASE_URL}/api/ai-search/products?query=laptop&limit=5`);
  check(searchResponse, {
    'ai search products returns 200': (response) => response.status === 200,
    'ai search products has a response body': (response) => Boolean(response.body) && response.body.length > 0,
  });
}

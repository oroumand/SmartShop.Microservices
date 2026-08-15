#!/usr/bin/env bash
set -euo pipefail

command -v curl >/dev/null || { echo "curl is required" >&2; exit 1; }
command -v jq >/dev/null || { echo "jq is required" >&2; exit 1; }

gateway_url="${GATEWAY_URL:-http://localhost:8088}"
customer_id="${CUSTOMER_ID:-22222222-2222-2222-2222-222222222222}"

restore_broker() {
  docker compose start rabbitmq >/dev/null 2>&1 || true
}
trap restore_broker EXIT

product_id=$(curl --silent --fail-with-body \
  "$gateway_url/api/catalog/products" | jq -er '.[0].id')
order_id=$(curl --silent --fail-with-body \
  -H 'Content-Type: application/json' \
  -d "{\"customerId\":\"$customer_id\",\"customerName\":\"Failure Demo\",\"customerEmail\":\"failure@smartshop.local\",\"items\":[{\"productId\":\"$product_id\",\"quantity\":1}]}" \
  "$gateway_url/api/orders" | jq -er '.id')

docker compose stop rabbitmq

payment_id=$(curl --silent --fail-with-body \
  -H 'Content-Type: application/json' \
  -d "{\"orderId\":\"$order_id\",\"method\":\"FakeGateway\"}" \
  "$gateway_url/api/payments" | jq -er '.id')

echo "Payment committed while RabbitMQ is down: $payment_id"
curl --silent --fail-with-body http://localhost:8082/ops/outbox | jq .

docker compose start rabbitmq
trap - EXIT

for _ in $(seq 1 45); do
  pending=$(curl --silent --fail-with-body \
    http://localhost:8082/ops/outbox | jq -er '.pendingCount')
  if [[ "$pending" -eq 0 ]]; then
    echo "Outbox drained after RabbitMQ recovered."
    curl --silent --fail-with-body \
      "$gateway_url/api/loyalty/customers/$customer_id" | jq .
    exit 0
  fi
  sleep 1
done

echo "Outbox did not drain in time." >&2
exit 1

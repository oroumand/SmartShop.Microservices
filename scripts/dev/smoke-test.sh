#!/usr/bin/env bash
set -euo pipefail

command -v curl >/dev/null || { echo "curl is required" >&2; exit 1; }
command -v jq >/dev/null || { echo "jq is required" >&2; exit 1; }

gateway_url="${GATEWAY_URL:-http://localhost:8088}"
customer_id="${CUSTOMER_ID:-11111111-1111-1111-1111-111111111111}"
correlation_id="smoke-$(date +%s)"

products=$(curl --silent --fail-with-body \
  -H "X-Correlation-ID: $correlation_id" \
  "$gateway_url/api/catalog/products")
product_id=$(jq -er '.[0].id' <<<"$products")

order=$(curl --silent --fail-with-body \
  -H 'Content-Type: application/json' \
  -H "X-Correlation-ID: $correlation_id" \
  -d "{\"customerId\":\"$customer_id\",\"customerName\":\"Demo Customer\",\"customerEmail\":\"demo@smartshop.local\",\"items\":[{\"productId\":\"$product_id\",\"quantity\":1}]}" \
  "$gateway_url/api/orders")
order_id=$(jq -er '.id' <<<"$order")

payment=$(curl --silent --fail-with-body \
  -H 'Content-Type: application/json' \
  -H "X-Correlation-ID: $correlation_id" \
  -d "{\"orderId\":\"$order_id\",\"method\":\"FakeGateway\"}" \
  "$gateway_url/api/payments")
payment_id=$(jq -er '.id' <<<"$payment")

for _ in $(seq 1 30); do
  account=$(curl --silent --fail-with-body \
    -H "X-Correlation-ID: $correlation_id" \
    "$gateway_url/api/loyalty/customers/$customer_id")
  balance=$(jq -er '.balance' <<<"$account")
  order_status=$(curl --silent --fail-with-body \
    -H "X-Correlation-ID: $correlation_id" \
    "$gateway_url/api/orders/$order_id" | jq -er '.status')

  if [[ "$balance" -gt 0 && "$order_status" == "Paid" ]]; then
    jq -n \
      --arg correlationId "$correlation_id" \
      --arg orderId "$order_id" \
      --arg paymentId "$payment_id" \
      --argjson loyaltyBalance "$balance" \
      '{correlationId:$correlationId,orderId:$orderId,paymentId:$paymentId,orderStatus:"Paid",loyaltyBalance:$loyaltyBalance}'
    exit 0
  fi

  sleep 1
done

echo "Timed out waiting for Ordering and Loyalty consumers." >&2
exit 1

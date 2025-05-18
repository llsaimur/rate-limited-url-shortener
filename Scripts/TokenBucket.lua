-- KEYS[1]: rate_limit:{clientId}
-- ARGV[1]: max_tokens
-- ARGV[2]: refill_rate (tokens per second)
-- ARGV[3]: current timestamp in seconds

local bucket = redis.call("HMGET", KEYS[1], "tokens", "last_refill")
local tokens = tonumber(bucket[1])
local last_refill = tonumber(bucket[2])

if tokens == nil then
  tokens = tonumber(ARGV[1])
  last_refill = tonumber(ARGV[3])
end

local now = tonumber(ARGV[3])
local elapsed = now - last_refill
local refill = elapsed * tonumber(ARGV[2])
tokens = math.min(tonumber(ARGV[1]), tokens + refill)

local allowed = 0
if tokens >= 1 then
  tokens = tokens - 1
  allowed = 1
end

redis.call("HMSET", KEYS[1], "tokens", tokens, "last_refill", now)
redis.call("EXPIRE", KEYS[1], 3600)

return allowed

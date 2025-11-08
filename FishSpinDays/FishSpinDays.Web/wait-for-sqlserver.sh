#!/bin/sh
set -e

host="$1"
shift
port="$1"
shift

until nc -z "$host" "$port"; do
 echo "Waiting for SQL Server at $host:$port..."
 sleep 2
done

echo "SQL Server is up - executing command"
exec "$@"

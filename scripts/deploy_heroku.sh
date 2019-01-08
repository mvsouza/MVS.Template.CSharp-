docker-compose build --force-rm
docker login --username=_ --password=$api_key registry.heroku.com
docker tag $docker_image_name registry.heroku.com/$docker_image_name/web
docker push registry.heroku.com/$docker_image_name/web
export imageid=$(docker inspect jsondiffer --format="{{.Id}}")
echo Image $imageid
export updateJson='{ "updates": [{ "type": "web", "docker_image": "'$imageid'"}] }'
curl -n -X PATCH https://api.heroku.com/apps/$heroku_name/formation   -d "$updateJson" -H "Content-Type: application/json" -H "Accept: application/vnd.heroku+json; version=3.docker-releases" -H "Authorization: Basic $heroku_auth"
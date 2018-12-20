docker-compose build --force-rm
docker login --username=_ --password=$api_key registry.heroku.com
docker tag MVS.Template.CSharp.API registry.heroku.com/mvs-template-csharp/web
docker push registry.heroku.com/mvs-template-csharp/web
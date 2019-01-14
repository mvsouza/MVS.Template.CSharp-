Github:
    First of all create a Github repository with your projects name and on setting get the project key.
Sonarcloud:
    On [setup manually](https://sonarcloud.io/projects/create) insert your account key and put the name of the project(MVS.Template.CSharp)
    Generate a token and save it for later
Opencover:
    Create a new project using the github repository and save the generated token.
Appveyor:
    Access appveyor and add your project from github.
    Add the enviroment variables:
      "codecov_token" with the token genareted by codecov
      "sonarcloud_login" with the token genareted on sonarcloud
      "sonarcloud_org" with your cloud org name
      "sonarcloud_key" with your project key
    Change the Build worker image to Visual Studio 2017
Heroku:
    Create a new app
    Get your heroku api-key on your profile
    If your app has a diferent name from your projet(MVS.Template.CSharp), chage the heroku script inside the folder scripts.
Travis:
    Turn travis-ci up for the repository
    Set the variable for heroku
      "api_key" with the heroku apikey
Travis: 
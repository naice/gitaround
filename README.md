# gitaround
A windows tool to get around SourceTree, but still use BitBuckets "Checkout in SourceTree" feature. 

# Configuration
You need to start debug_update_registry.cmd with elevated permission, it will delete the sourcetree protocol linking and will 
replace it with itself. 

The configuration is done in gitaround-config.json. You need to define Repositories and SshCredentials, they will be linked by the User field.
All fields are mandatory for the service to work. If you dont got a passphrase for your ssh keys just fill it with anything you like.

```json
{
  "LogFilePath": "%APPPATH%\\gitaround.log",
  "Repositories": [
    {
      "Local": "C:\\Path_to_your_project",
      "CloneUrl": "ssh://git@clone.url/project.git",
      "Remote": "origin",
      "User": "jmarchewka"
    }
  ],
  "SshCredentials": [
    {
      "Passphrase" : "idk",
      "PublicKeyFile":  "C:\\Users\\jmarchewka\\ssh\\public.pub",
      "PrivateKeyFile": "C:\\Users\\jmarchewka\\ssh\\private",
      "User": "jmarchewka"
    }
  ]
}
```

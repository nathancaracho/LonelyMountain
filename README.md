# Lonely Mountain
[Lonely Mountain](http://tolkiengateway.net/wiki/Lonely_Mountain) is a worker library and consumer template. 

## Todo
- [ ] Add Template Creator
    - [x] Create template
    - [ ] Add packer to template
- [ ] Add appsetting injection 
- [ ] Add Service bus supplier
- [ ] Add Tests
- [ ] Create packer to consumer lib
- [ ] Fix example
    - [ ] Add K8 example
    - [x] Add worker folder
- [x] Add project structure

## Project structure
``` text
ROOT \
┣ LonelyMountain.Src \
┃ ┣ 📂 consumer \
┃ ┣ 📂 Ioc \
┃ ┣ 📂 Queue \
┃ ┣ 📂 Subscriber \
┃ ┣ ┣  Bootstrap.cs 
┃ ┣ ┣  Worker.cs 
┃ ┗ ┗  LonelyMountain.Src.csproj
┃
┣ LonelyMountain.Template \
┃ ┣ 📂 templates \
┃ ┣ 📂 Consumer \
┃ ┣ ┣ 📂 .template.config \
┃ ┣ ┣ ┣  ┗ LonelyMountain.Src.csproj
┃ ┣ ┣ ┣  projectNameConsumer.cs 
┃ ┣ ┣ ┣  projectNameMessage.cs 
┃ ┣ ┣ ┣  projectNameValidator.cs 
┃ ┣ ┣ ┗  program.cs
┃ ┗  LonelyMountain.Template.csproj
┃
┣ LonelyMountain.Example \
┃ ┣ 📂 Worker \
┃ ┣ ┣ 📂 Customer \
┃ ┣ ┣ ┣  LonelyMountain.Example.csproj
┃ ┣ ┣ ┣  CustomerConsumer.cs 
┃ ┣ ┣ ┣  CustomerMessage.cs 
┃ ┣ ┣ ┣  CustomerValidator.cs 
┃ ┗ ┗ ┗  program.cs
┃
┣  docker-compose.yml
┣  README.md
┗  LonelyMountain.sln
```
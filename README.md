# Unity 3D RPG 游戏开发
Study with M_Studio RPG Course 

### [b站游戏演示](https://www.bilibili.com/video/BV1y14y177pp)
### 这个游戏是跟着M_Studio的游戏教程进行开发的 [教程链接在这里](https://www.bilibili.com/video/BV1rf4y1k7vE/?spm_id_from=333.788)

* 游戏的实现细节与原教程有所不同，如敌人状态的逻辑实现使用了FSM，攻击方式有区别，玩家增加了盾反功能等
* 实现了核心功能，玩家的攻击防御，怪物的攻击防御，数值更新等
* 因为是以学为目的的项目，会有较多的代码注释，欢迎访问 [我的主页](https://www.kaku36.com) 查看相应的教程笔记
* 付费内容背包系统，音效，任务系统，对话系统已添加，因为付费内容源码暂时还没放上来

### 使用方法
* 将该项目下载到本地，将ProjectSettings和Assets放在同一文件夹下
* 该项目使用的是Unity 2020.3版本进行的开发，在UnityHub中添加上述文件夹
* 在项目中安装Universal RP包以及Cinemachine包
* Edit → ProjectSettings → Graphics里面将渲染管线替换为文件夹中的Universal Render Pipeline
* 在Build Settings里将MainMenu，Scene1，Scene2，Scene3四个场景添加进去，然后返回MainMenu就可以顺利运行了

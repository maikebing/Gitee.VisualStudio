# 码云的 Visual Studio 扩展

[![Build status](https://ci.appveyor.com/api/projects/status/calues98sxdfnt45?svg=true)](https://ci.appveyor.com/project/MaiKeBing/gitee-visualstudio)
[![Nuget Version](https://img.shields.io/nuget/v/Gitee.Api.svg)](https://www.nuget.org/packages/Gitee.Api/)
![Visual Studio Marketplace Version](https://img.shields.io/visual-studio-marketplace/v/Gitee.GiteeVisualStudio.svg?label=Visual%20Studio%20Marketplace%20Version)
![Visual Studio Marketplace Installs](https://img.shields.io/visual-studio-marketplace/i/Gitee.GiteeVisualStudio.svg?label=Visual%20Studio%20Marketplace%20Installs)
![Visual Studio Marketplace Downloads](https://img.shields.io/visual-studio-marketplace/d/Gitee.GiteeVisualStudio.svg?label=Visual%20Studio%20Marketplace%20Downloads)
![Visual Studio Marketplace Rating](https://img.shields.io/visual-studio-marketplace/r/Gitee.GiteeVisualStudio.svg?label=Visual%20Studio%20Marketplace%20Rating)


提示 Visual Studio  2015,2017,2019 均有社区版，码云的 Visual Studio 扩展支持 2015、2017、2019、的社区版、专业版、企业版。

社区版基本功能和专业版一致，前提是只能用于非商业项目和开源项目。Express 版可以用于商业项目，也可以只下载 Visual Studio C++ Build Tools ，这个可以无限制使用。

## 安装

* 当前发布版本 [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=Gitee.GiteeVisualStudio)
* 最新构建版本 [Open VSIX Gallery](http://vsixgallery.com/extension/B1077C66-1666-4F60-BDFA-BA078FDABCCE/)  

注：Gitee.VisualStudio 只支持 Visual Studio 2015/2017/2019。

## 使用

Visual Studio 版本管理相关功能，都集中在 Team Explorer Gitee.VisualStudio 的各个功能，都是穿插在 Team Explorer 的工作流中。

### Connect页

打开 Visual Studio 之后，展开 Team Explorer 面板，默认是 Connect 页（也可以在 Team Explorer 面板的工具栏中，点击插头图标，跳转到 Connect 页).  

如果用户尚未登陆码云, 可以在 Connect 页的 Gitee 区中，点击连接按钮登陆；若尚未注册码云，可以在该区中，点击注册按钮，在码云网站进行注册。  

用户登陆码云之后，Gitee 区中，会显示已经通过 Visual Studio 克隆到本地的码云项目。可以在 Gitee 区的工具栏进行克隆、克隆和退出操作。双击该区中项目列表的项目，可以打开该项目。

#### 克隆

在 Gitee 区的工具栏中，点击克隆按钮，会弹出当前用户的所有码云项目，包括所属组的项目。选择其中一个，点击下方克隆按钮即可。

注：默认情况下，所选项目会被克隆到 %USERPROFILE%\Source\Repos 下，可以通过下方浏览按钮修改 clone 路径。若该目录下有和项目同名的文件夹存在，克隆按钮将不可用。

#### 创建项目

在 Gitee 区的工具栏中，点击创建项目按钮，在弹出的对话框中，填写项目名称, 描述，选择 Git ignore 和协议，项目路径之后，点击下方创建按钮即可。

#### 退出

在 Gitee 区的工具栏中，点击退出按钮，即可退出当前用户。

注：登陆信息会从系统清除，但是克隆的项目仍旧保留，下次登陆仍旧存在。

#### 推送到码云
在 Team Explorer 的 Synchronization/Publish 页中，通过其中的码云区，可以将当前Git项目发布到码云（需要填写项目名，描述，协议，以及所有权）。若当前项目不是Git项目，可以在VisualStudio右下角，点击Add to Source Control / Git，将其转化为Git项目，然后再发布。

#### 提交更改

Team Explorer 本身集成了 Commit，pull，sync 等 Git 操作，可以通过 Team Explorer 工具栏的小房子按钮，跳转到Home页进行相关操作。 Gitee.VisualStudio插件将码云特有的Attachment，Pull Request，Issues，Statistics，Wiki功能，都集成到该页面，通过点击相关按钮，跳转到对应的页面。

## 演示动画

![image](./docs/images/option.gif)

## 许可协议及历史

本软件最初采用悬赏的方式由 @dilly 开发，代码借鉴了 Github for Visual Studio 源码，对此对所有相关的开发者表示感谢。

# Addin

Addin 是一个基于 dotnet appdomain 插件框架的实现。 

该框架可以实现对一个或多个dll组成的插件进行动态的加载、发布，并自动为插件接口生成文档。

## 特点

因为使用了应用程序域（AppDomain）技术，插件额运行环境是相互隔离的，一个插件down掉不会影响其他插件。

插件上传者可以在web端对插件进行上传、加载、启用、禁用等操作

因为dotnet是基于CLR的，CLR是一个混合编程的平台，只要一门语言有面向CLR的编译器，这门语言就可以编译成中间语言。故：Addin 理论上可以承载其他语言编译生成的插件。（本人未实际测试）

## 原理

参照 dotnet core mvc 实现了一个简单的 mvc 框架，mvc的路由与dll操作层挂接。



# 服务器架构学习

学习高并发、高Cpu利用率的分布式服务器（MOBA）


## 主要功能

1.使用libuv处理异步操作，例如：文件io，工作队列，计时器等。

2.内置lua解释器，导出C函数接口。可以支持纯Lua开发以及C++、lua混合开发

3.自定义包结构，包体数据支持json和protobuf

4.支持Mysql及Redis数据库

5.异步日志文件输出

6.支持tcp及udp

7.分布式部署各功能模块服务器

8.同步方式为基于UDP的帧同步

## 总体架构
![image1](https://github.com/FacelessXcy/moba_game_server/blob/master/Image/%E6%9C%8D%E5%8A%A1%E5%99%A8%E6%80%BB%E4%BD%93%E6%9E%B6%E6%9E%84.png)


## 网关数据转发细节
![image2](https://github.com/FacelessXcy/moba_game_server/blob/master/Image/网关数据转发.png)



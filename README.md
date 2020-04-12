# 服务器架构学习

学习高并发、高Cpu利用率的分布式服务器（MOBA）


## 主要功能

1.使用libuv处理异步操作，例如：文件io，工作队列，计时器等。

2.内置lua解释器，导出C函数接口。可以支持纯Lua开发以及C++、lua混合开发

3.自定义包结构，包体数据支持json和protobuf

4.支持Mysql及Redis数据库

5.异步日志文件输出

6.支持tcp及udp

6.学习中...

## 总体架构
![image]（https://github.com/FacelessXcy/moba_game_server/Image/服务器总体架构.png）

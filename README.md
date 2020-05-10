# 服务器以及帧同步下的客户端逻辑学习

学习分布式服务器架构
以及在帧同步下，客户端的同步逻辑（MOBA）


## 主要功能

1.使用libuv处理异步操作，例如：文件io，工作队列，计时器等。

2.内置lua解释器，导出C函数接口。可以支持纯Lua开发以及C++、lua混合开发

3.自定义包结构，包体数据支持json和protobuf

4.支持Mysql及Redis数据库

5.异步日志文件输出

6.支持tcp及udp

7.分布式部署各功能模块服务器

8.同步方式为基于UDP的帧同步

9.使用UDP实现一定程度上的可靠传输
## 总体架构
![image1](https://github.com/FacelessXcy/moba_game_server/blob/master/Image/%E6%9C%8D%E5%8A%A1%E5%99%A8%E6%80%BB%E4%BD%93%E6%9E%B6%E6%9E%84.png)


## 网关数据转发细节
![image2](https://github.com/FacelessXcy/moba_game_server/blob/master/Image/网关数据转发.png)


##帧同步步骤
1.开始，由服务器向客户端发送帧数据包，驱动客户端开始调用帧同步循环LogicUpdate()

2.每次循环中:

       (1)同步_lastFrameOpts的逻辑操作，调整位置到真实的逻辑位置:
            调整完以后，客户端同步到syncFrameID
        
       (2)从syncFrameID+1开始-->frame.frameid-1
           同步丢失的帧，快速同步到当前帧
           所有客户端的数据都被同步到frame.frameid-1
           同步这些帧造成的由帧驱动的公共物体的位置(如小兵位置)
 
      (3)获取最后一个操作 frame.frameid的操作，同时syncFrameID=frame,frameid
          更新_lastFrameOpts为该帧
          根据这个帧来处理，更新动画状态以及位移，产生的位移为“假位移”

      (4)采集下一帧要发送给服务器的操作




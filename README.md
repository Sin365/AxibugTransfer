# AxibugTransfer

一个自制的开源端口转发工具  By 皓月

dotNet 版本: .Net 6 

平台: Windows/Linux/Other..

项目"AxibugTransfer"为控制台版本项目

项目"AxibugTransfer.Tcp"为TCP端口转发库


【控制台版本使用方法】

下载Release版本，或者自编译AxibugTransfer项目.
编辑可执行程序目录的config.cfg配置文件

格式（可多行）
本地监听端口:目标IP:目标端口

如：

	80:1.2.3.4:8080
	81:1.2.3.4:8081

表示

将本地80端口TCP/HTTP请求转发到1.2.3.4的8080端口上

将本地81端口 转发到1.2.3.4的8081端口上

然后启动可执行程序 AxibugTransfer

你也可以设为开机自启，配置成服务啥的

【TODO】

	完善的统计功能

	使用 IOCP/epoll 提升性能

	也许做一个GUI版本
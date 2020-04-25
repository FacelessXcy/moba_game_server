-- --------------------------------------------------------
-- 主机:                           127.0.0.1
-- 服务器版本:                        8.0.19 - MySQL Community Server - GPL
-- 服务器操作系统:                      Win64
-- HeidiSQL 版本:                  11.0.0.5919
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- 导出 auth_center 的数据库结构
CREATE DATABASE IF NOT EXISTS `auth_center` /*!40100 DEFAULT CHARACTER SET utf8 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `auth_center`;

-- 导出  表 auth_center.uinfo 结构
CREATE TABLE IF NOT EXISTS `uinfo` (
  `uid` int unsigned NOT NULL AUTO_INCREMENT COMMENT '玩家唯一标识',
  `unick` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '""' COMMENT '玩家昵称',
  `usex` int NOT NULL DEFAULT '0' COMMENT '0男；1女',
  `uface` int NOT NULL DEFAULT '0' COMMENT '系统默认头像',
  `uname` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '""' COMMENT '玩家账号名',
  `upwd` varchar(32) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '""' COMMENT '玩家密码的MD5值',
  `phone` varchar(16) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '""' COMMENT '玩家电话',
  `email` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '""' COMMENT '玩家email',
  `address` varchar(128) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '""' COMMENT '玩家地址',
  `uvip` int NOT NULL DEFAULT '0' COMMENT '玩家VIP等级',
  `vip_end_time` int NOT NULL DEFAULT '0' COMMENT '玩家VIP到期时间戳',
  `is_guest` int NOT NULL DEFAULT '0' COMMENT '标志位,是否为游客账号',
  `guest_key` varchar(64) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL DEFAULT '0' COMMENT '游客账号标识',
  `status` int NOT NULL DEFAULT '0' COMMENT '0正常',
  PRIMARY KEY (`uid`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='玩家信息表';

-- 数据导出被取消选择。

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

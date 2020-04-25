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


-- 导出 moba_game 的数据库结构
CREATE DATABASE IF NOT EXISTS `moba_game` /*!40100 DEFAULT CHARACTER SET utf8 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `moba_game`;

-- 导出  表 moba_game.ugame 结构
CREATE TABLE IF NOT EXISTS `ugame` (
  `id` int NOT NULL AUTO_INCREMENT COMMENT '全局唯一ID',
  `uid` int NOT NULL COMMENT '用户UID',
  `uchip` int NOT NULL DEFAULT '0' COMMENT '用户金币数目',
  `uchip2` int NOT NULL DEFAULT '0' COMMENT '其他的货币或等价物',
  `uchip3` int NOT NULL DEFAULT '0' COMMENT '其他的货币或等价物',
  `uvip` int NOT NULL DEFAULT '0' COMMENT '在本游戏中的vip等级',
  `uvip_endtime` int NOT NULL DEFAULT '0' COMMENT 'vip结束时间',
  `udata1` int NOT NULL DEFAULT '0' COMMENT '用户的道具1',
  `udata2` int NOT NULL DEFAULT '0' COMMENT '用户的道具2',
  `udata3` int NOT NULL DEFAULT '0' COMMENT '用户的道具3',
  `uexp` int NOT NULL DEFAULT '0' COMMENT '用户经验值',
  `ustatus` int NOT NULL DEFAULT '0' COMMENT '是否被封号.0表示正常',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='存放玩家在moba游戏中的游戏数据：\r\n金币，其他货币，道具，游戏中的vip等级，账号状态，玩家经验\r\nuid来标识玩家，id作为自增长的唯一标识';

-- 数据导出被取消选择。

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;

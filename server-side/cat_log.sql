/*
 Navicat Premium Data Transfer

 Source Server         : local mysql80
 Source Server Type    : MySQL
 Source Server Version : 80013
 Source Host           : localhost:3306
 Source Schema         : cat_log

 Target Server Type    : MySQL
 Target Server Version : 80013
 File Encoding         : 65001

 Date: 03/03/2020 12:31:45
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for t_articles
-- ----------------------------
DROP TABLE IF EXISTS `t_articles`;
CREATE TABLE `t_articles`  (
  `article_id` bigint(255) NOT NULL AUTO_INCREMENT COMMENT '文章ID',
  `article_title` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '标题',
  `article_content` longtext CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '内容',
  `article_introduction` text CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '引言',
  `article_views_count` bigint(20) NOT NULL DEFAULT 0 COMMENT '浏览量',
  `article_time` datetime(0) NOT NULL COMMENT '发表时间',
  `article_like_count` bigint(20) NOT NULL DEFAULT 0 COMMENT '点赞数',
  `column_id` bigint(20) NOT NULL COMMENT '栏目ID',
  PRIMARY KEY (`article_id`) USING BTREE,
  INDEX `column_id`(`column_id`) USING BTREE,
  CONSTRAINT `t_articles_ibfk_1` FOREIGN KEY (`column_id`) REFERENCES `t_columns` (`column_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_articles
-- ----------------------------
INSERT INTO `t_articles` VALUES (10, '什么是 WPF？', 'Windows Presentation Foundation（WPF）是美国微软公司推出.NET Framework 3.0及以后版本的组成部分之一，它是一套基于XML、.NET Framework、向量绘图技术的展示层开发框架，微软视其为下一代用户界面技术，广泛被用于Windows Vista的界面开发。\r\n\r\nWPF使用一种新的XAML（eXtensible Application Markup Language）语言来开发界面，这将把界面开发以及后台逻辑很好的分开，降低了耦合度，使用户界面设计师与程序开发者能更好的合作，降低维护和更新的成本。\r\n\r\nWPF/E是WPF的子集合，全名是：Windows Presentation Foundation Everywhere。在基于XAML与JavaScript之下，可跨越各种平台，当前WPF/E已演化为Microsoft Silverlight，担负微软在丰富互联网应用程序领域，并正面与Adobe Flash竞争的产品。', 'WPF（Windows Presentation Foundation）是微软推出的基于Windows 的用户界面框架，属于. NET Framework 3.0的一部分。', 0, '2020-03-03 15:22:22', 0, 11);
INSERT INTO `t_articles` VALUES (11, '什么是 .NET Core', '.NET Core 是.NET Framework的新一代版本，是微软开发的第一个跨平台 (Windows、Mac OSX、Linux) 的应用程序开发框架（Application Framework），未来也将会支持 FreeBSD 与 Alpine 平台。.Net Core也是微软在一开始发展时就开源的软件平台[1]，它经常也会拿来和现有的开源 .NET 平台 Mono 比较。\r\n\r\n由于 .NET Core 的开发目标是跨平台的 .NET 平台，因此 .NET Core 会包含 .NET Framework 的类别库，但与 .NET Framework 不同的是 .NET Core 采用包化 (Packages) 的管理方式，应用程序只需要获取需要的组件即可，与 .NET Framework 大包式安装的作法截然不同，同时各包亦有独立的版本线 (Version line)，不再硬性要求应用程序跟随主线版本。', '.NET Core 是.NET Framework的新一代版本，是微软开发的第一个跨平台的应用程序开发框架', 0, '2020-03-04 13:29:59', 0, 10);
INSERT INTO `t_articles` VALUES (12, '什么是 JVM？', 'Java虚拟机（英语：Java Virtual Machine，缩写为JVM），一种能够运行Java bytecode的虚拟机，以堆栈结构机器来进行实做。最早由太阳微系统所研发并实现第一个实现版本，是Java平台的一部分，能够运行以Java语言写作的软件程序。\r\n\r\nJava虚拟机有自己完善的硬体架构，如处理器、堆栈、寄存器等，还具有相应的指令系统。JVM屏蔽了与具体操作系统平台相关的信息，使得Java程序只需生成在Java虚拟机上运行的目标代码（字节码），就可以在多种平台上不加修改地运行。通过对中央处理器（CPU）所执行的软件实现，实现能执行编译过的Java程序码（Applet与应用程序）。\r\n\r\n作为一种编程语言的虚拟机，实际上不只是专用于Java语言，只要生成的编译文件符合JVM对加载编译文件格式要求，任何语言都可以由JVM编译运行。此外，除了甲骨文，也有其他开源或闭源的实现。', 'Java虚拟机（英语：Java Virtual Machine，缩写为JVM），一种能够运行Java bytecode的虚拟机，以堆栈结构机器来进行实做。', 0, '2020-03-04 15:30:48', 0, 12);
INSERT INTO `t_articles` VALUES (13, '.NET Core 运行时前滚策略更新', '现在，.NET Core 运行时（更具体地讲，运行时绑定程序）开始提供主版本的前滚选择策略。运行时绑定程序将默认启用面向补丁与次版本的前滚功能。我们决定公开一组更为广泛的策略，以帮助开发人员快速恢复可能存在的更新问题，但原有前滚操作并不受影响。\r\n\r\n我们发布了一项名为 RollForward 的新属性，该属性能够接受以下值：\r\n\r\nLatestPatch——前滚至最新补丁版本。其会禁用 Minor 策略。\r\nMinor ——前滚至最早次版本以解决所需次版本缺失问题。如果该次版本存在，则 LatesPatch 策略即可起效。这也是系统中采用的默认策略。\r\nMajor ——前滚至最早主版本与最早次版本，以解决所需主版本缺失问题。如果该主版本存在，则随后使用 Minor 策略。\r\nLatestMinor——前滚至最新次版本，即使存在所请求的次版本亦不受影响。\r\nLatestMajor ——前滚至最新主版本，即使存在所请求的主版本亦不受影响。\r\nDisable ——不进行前滚。仅绑定至指定版本。我们不建议大家在常规用途中使用这一选项，因为其会禁用前滚至最新补丁后版本的功能。仅建议您在测试环境中使用。', '现在，.NET Core 运行时（更具体地讲，运行时绑定程序）开始提供主版本的前滚选择策略。运行时绑定程序将默认启用面向补丁与次版本的前滚功能。', 0, '2020-03-04 15:40:09', 0, 10);

-- ----------------------------
-- Table structure for t_columns
-- ----------------------------
DROP TABLE IF EXISTS `t_columns`;
CREATE TABLE `t_columns`  (
  `column_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '栏目ID',
  `column_name` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '名称',
  `column_description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '描述',
  `section_id` bigint(20) NOT NULL COMMENT '板块ID',
  PRIMARY KEY (`column_id`) USING BTREE,
  INDEX `section_id`(`section_id`) USING BTREE,
  INDEX `column_name`(`column_name`) USING BTREE,
  CONSTRAINT `t_columns_ibfk_1` FOREIGN KEY (`section_id`) REFERENCES `t_sections` (`section_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_columns
-- ----------------------------
INSERT INTO `t_columns` VALUES (10, 'Core 3.x', '.NET Core 3.x 的相关文章', 10);
INSERT INTO `t_columns` VALUES (11, 'WPF', 'WPF 框架的相关文章', 10);
INSERT INTO `t_columns` VALUES (12, 'JVM', 'Java 虚拟机的相关文章', 11);

-- ----------------------------
-- Table structure for t_sections
-- ----------------------------
DROP TABLE IF EXISTS `t_sections`;
CREATE TABLE `t_sections`  (
  `section_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '板块ID',
  `section_name` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '名称',
  `section_description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '描述',
  PRIMARY KEY (`section_id`) USING BTREE,
  INDEX `section_name`(`section_name`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of t_sections
-- ----------------------------
INSERT INTO `t_sections` VALUES (10, '.NET', '.NET Framework 专题');
INSERT INTO `t_sections` VALUES (11, 'JAVA', 'JAVA 专题');

-- ----------------------------
-- Table structure for t_tags
-- ----------------------------
DROP TABLE IF EXISTS `t_tags`;
CREATE TABLE `t_tags`  (
  `tag_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '标签ID',
  `tag_name` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '名称',
  `tag_description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '描述',
  PRIMARY KEY (`tag_id`) USING BTREE,
  INDEX `tag_name`(`tag_name`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_tags_artitles
-- ----------------------------
DROP TABLE IF EXISTS `t_tags_artitles`;
CREATE TABLE `t_tags_artitles`  (
  `tags_artitles_id` bigint(255) NOT NULL AUTO_INCREMENT,
  `tag_id` bigint(20) NOT NULL COMMENT '标签ID',
  `article_id` bigint(255) NOT NULL COMMENT '文章ID',
  PRIMARY KEY (`tags_artitles_id`) USING BTREE,
  INDEX `tag_id`(`tag_id`) USING BTREE,
  INDEX `article_id`(`article_id`) USING BTREE,
  CONSTRAINT `t_tags_artitles_ibfk_1` FOREIGN KEY (`tag_id`) REFERENCES `t_tags` (`tag_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `t_tags_artitles_ibfk_2` FOREIGN KEY (`article_id`) REFERENCES `t_articles` (`article_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

SET FOREIGN_KEY_CHECKS = 1;

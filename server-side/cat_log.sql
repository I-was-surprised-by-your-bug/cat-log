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
  `article_views_count` bigint(20) NOT NULL DEFAULT 0 COMMENT '浏览量',
  `article_time` datetime(0) NOT NULL COMMENT '发表时间',
  `article_like_count` bigint(20) NOT NULL DEFAULT 0 COMMENT '点赞数',
  PRIMARY KEY (`article_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_column_artitles
-- ----------------------------
DROP TABLE IF EXISTS `t_column_artitles`;
CREATE TABLE `t_column_artitles`  (
  `column_artitles_id` bigint(255) NOT NULL AUTO_INCREMENT,
  `column_id` bigint(20) NOT NULL COMMENT '栏目ID',
  `article_id` bigint(255) NOT NULL COMMENT '文章ID',
  PRIMARY KEY (`column_artitles_id`) USING BTREE,
  INDEX `column_id`(`column_id`) USING BTREE,
  INDEX `article_id`(`article_id`) USING BTREE,
  CONSTRAINT `t_column_artitles_ibfk_1` FOREIGN KEY (`column_id`) REFERENCES `t_columns` (`column_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `t_column_artitles_ibfk_2` FOREIGN KEY (`article_id`) REFERENCES `t_articles` (`article_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_columns
-- ----------------------------
DROP TABLE IF EXISTS `t_columns`;
CREATE TABLE `t_columns`  (
  `column_id` bigint(20) NOT NULL AUTO_INCREMENT COMMENT '栏目ID',
  `column_name` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '名称',
  `column_description` text CHARACTER SET utf8 COLLATE utf8_general_ci NULL COMMENT '描述',
  PRIMARY KEY (`column_id`) USING BTREE,
  INDEX `column_name`(`column_name`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Table structure for t_section_columns
-- ----------------------------
DROP TABLE IF EXISTS `t_section_columns`;
CREATE TABLE `t_section_columns`  (
  `section_columns_id` bigint(20) NOT NULL AUTO_INCREMENT,
  `section_id` bigint(20) NOT NULL COMMENT '板块ID',
  `column_id` bigint(20) NOT NULL COMMENT '栏目ID',
  PRIMARY KEY (`section_columns_id`) USING BTREE,
  INDEX `section_id`(`section_id`) USING BTREE,
  INDEX `column_id`(`column_id`) USING BTREE,
  CONSTRAINT `t_section_columns_ibfk_1` FOREIGN KEY (`section_id`) REFERENCES `t_sections` (`section_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `t_section_columns_ibfk_2` FOREIGN KEY (`column_id`) REFERENCES `t_columns` (`column_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

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

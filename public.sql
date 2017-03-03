/*
Navicat PGSQL Data Transfer

Date: 2017-03-03 11:53:01
*/


-- ----------------------------
-- Sequence structure for mroom_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."mroom_id_seq";
CREATE SEQUENCE "public"."mroom_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for mroomtag_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."mroomtag_id_seq";
CREATE SEQUENCE "public"."mroomtag_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Sequence structure for mtype_id_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."mtype_id_seq";
CREATE SEQUENCE "public"."mtype_id_seq"
 INCREMENT 1
 MINVALUE 1
 MAXVALUE 9223372036854775807
 START 1
 CACHE 1;

-- ----------------------------
-- Table structure for mroom
-- ----------------------------
DROP TABLE IF EXISTS "public"."mroom";
CREATE TABLE "public"."mroom" (
"id" int4 DEFAULT nextval('mroom_id_seq'::regclass) NOT NULL,
"mtype_id" int4,
"name" varchar(255) COLLATE "default",
"reason" text COLLATE "default",
"username" varchar(255) COLLATE "default",
"number" int2,
"state" "public"."et_mroomstate"
)
WITH (OIDS=FALSE)

;
COMMENT ON COLUMN "public"."mroom"."mtype_id" IS '分类';
COMMENT ON COLUMN "public"."mroom"."name" IS '会议室名称';
COMMENT ON COLUMN "public"."mroom"."reason" IS '描述';
COMMENT ON COLUMN "public"."mroom"."number" IS '容纳人数';
COMMENT ON COLUMN "public"."mroom"."state" IS '状态';

-- ----------------------------
-- Table structure for mroom_mroomtag
-- ----------------------------
DROP TABLE IF EXISTS "public"."mroom_mroomtag";
CREATE TABLE "public"."mroom_mroomtag" (
"mroom_id" int4 NOT NULL,
"mroomtag_id" int4 NOT NULL
)
WITH (OIDS=FALSE)

;

-- ----------------------------
-- Table structure for mroomtag
-- ----------------------------
DROP TABLE IF EXISTS "public"."mroomtag";
CREATE TABLE "public"."mroomtag" (
"id" int4 DEFAULT nextval('mroomtag_id_seq'::regclass) NOT NULL,
"name" varchar(255) COLLATE "default"
)
WITH (OIDS=FALSE)

;
COMMENT ON COLUMN "public"."mroomtag"."name" IS '标签名';

-- ----------------------------
-- Table structure for mtype
-- ----------------------------
DROP TABLE IF EXISTS "public"."mtype";
CREATE TABLE "public"."mtype" (
"id" int4 DEFAULT nextval('mtype_id_seq'::regclass) NOT NULL,
"parent_id" int4,
"name" varchar(255) COLLATE "default"
)
WITH (OIDS=FALSE)

;
COMMENT ON COLUMN "public"."mtype"."parent_id" IS '父';
COMMENT ON COLUMN "public"."mtype"."name" IS '类名称';

-- ----------------------------
-- Table structure for reserve
-- ----------------------------
DROP TABLE IF EXISTS "public"."reserve";
CREATE TABLE "public"."reserve" (
"id" int4 NOT NULL,
"mroom_id" int4,
"username" varchar(255) COLLATE "default",
"datetime1" timestamp(6),
"use_minute" int2,
"create_time" timestamp(6)
)
WITH (OIDS=FALSE)

;
COMMENT ON COLUMN "public"."reserve"."mroom_id" IS '会议室';
COMMENT ON COLUMN "public"."reserve"."username" IS '预约人';
COMMENT ON COLUMN "public"."reserve"."datetime1" IS '预约时间';
COMMENT ON COLUMN "public"."reserve"."use_minute" IS '使用时间(分钟)';
COMMENT ON COLUMN "public"."reserve"."create_time" IS '创建时间';

-- ----------------------------
-- Alter Sequences Owned By 
-- ----------------------------
ALTER SEQUENCE "public"."mroom_id_seq" OWNED BY "mroom"."id";
ALTER SEQUENCE "public"."mroomtag_id_seq" OWNED BY "mroomtag"."id";
ALTER SEQUENCE "public"."mtype_id_seq" OWNED BY "mtype"."id";

-- ----------------------------
-- Primary Key structure for table mroom
-- ----------------------------
ALTER TABLE "public"."mroom" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table mroom_mroomtag
-- ----------------------------
ALTER TABLE "public"."mroom_mroomtag" ADD PRIMARY KEY ("mroom_id", "mroomtag_id");

-- ----------------------------
-- Primary Key structure for table mroomtag
-- ----------------------------
ALTER TABLE "public"."mroomtag" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table mtype
-- ----------------------------
ALTER TABLE "public"."mtype" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table reserve
-- ----------------------------
ALTER TABLE "public"."reserve" ADD PRIMARY KEY ("id");

-- ----------------------------
-- Foreign Key structure for table "public"."mroom"
-- ----------------------------
ALTER TABLE "public"."mroom" ADD FOREIGN KEY ("mtype_id") REFERENCES "public"."mtype" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."mroom_mroomtag"
-- ----------------------------
ALTER TABLE "public"."mroom_mroomtag" ADD FOREIGN KEY ("mroomtag_id") REFERENCES "public"."mroomtag" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;
ALTER TABLE "public"."mroom_mroomtag" ADD FOREIGN KEY ("mroom_id") REFERENCES "public"."mroom" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."mtype"
-- ----------------------------
ALTER TABLE "public"."mtype" ADD FOREIGN KEY ("parent_id") REFERENCES "public"."mtype" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table "public"."reserve"
-- ----------------------------
ALTER TABLE "public"."reserve" ADD FOREIGN KEY ("mroom_id") REFERENCES "public"."mroom" ("id") ON DELETE NO ACTION ON UPDATE NO ACTION;

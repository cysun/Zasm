﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Classes" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" character varying(32) NOT NULL,
    CONSTRAINT "PK_Classes" PRIMARY KEY ("Id")
);

CREATE TABLE "Lessons" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "DateTime" timestamp without time zone NOT NULL,
    "Minutes" integer NOT NULL,
    "ClassId" integer NOT NULL,
    CONSTRAINT "PK_Lessons" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Lessons_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES "Classes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Students" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "Name" character varying(32) NOT NULL,
    "BirthYear" integer NULL,
    "ClassId" integer NOT NULL,
    "ParentName" character varying(32) NULL,
    "ParentEmail" character varying(255) NULL,
    "IsActivie" boolean NOT NULL,
    CONSTRAINT "PK_Students" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Students_Classes_ClassId" FOREIGN KEY ("ClassId") REFERENCES "Classes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Attendance" (
    "LessonId" integer NOT NULL,
    "StudentId" integer NOT NULL,
    CONSTRAINT "PK_Attendance" PRIMARY KEY ("LessonId", "StudentId"),
    CONSTRAINT "FK_Attendance_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES "Lessons" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Attendance_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Students" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Payments" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "DateTime" timestamp without time zone NOT NULL,
    "StudentId" integer NOT NULL,
    "Amount" integer NOT NULL,
    "Lessons" integer NOT NULL,
    CONSTRAINT "PK_Payments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Payments_Students_StudentId" FOREIGN KEY ("StudentId") REFERENCES "Students" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Attendance_StudentId" ON "Attendance" ("StudentId");

CREATE INDEX "IX_Lessons_ClassId" ON "Lessons" ("ClassId");

CREATE INDEX "IX_Payments_StudentId" ON "Payments" ("StudentId");

CREATE INDEX "IX_Students_ClassId" ON "Students" ("ClassId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20210204214540_InitialSchema', '5.0.2');

COMMIT;


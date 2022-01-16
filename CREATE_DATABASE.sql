--
-- PostgreSQL database dump
--

-- Dumped from database version 13.1
-- Dumped by pg_dump version 13.1

-- Started on 2022-01-16 16:42:09

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE "ORTest";
--
-- TOC entry 3035 (class 1262 OID 24670)
-- Name: ORTest; Type: DATABASE; Schema: -; Owner: postgres
--

CREATE DATABASE "ORTest" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'German_Germany.1252';


ALTER DATABASE "ORTest" OWNER TO postgres;

\connect "ORTest"

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 204 (class 1259 OID 24771)
-- Name: class; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.class (
    id bigint NOT NULL,
    name text,
    fk_teacher bigint
);


ALTER TABLE public.class OWNER TO postgres;

--
-- TOC entry 202 (class 1259 OID 24718)
-- Name: course; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.course (
    id bigint NOT NULL,
    name text NOT NULL,
    fk_teacher bigint
);


ALTER TABLE public.course OWNER TO postgres;

--
-- TOC entry 206 (class 1259 OID 24812)
-- Name: locks; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.locks (
    jclass character varying(48) NOT NULL,
    jobject character varying(48) NOT NULL,
    jtime timestamp without time zone NOT NULL,
    jowner character varying(48) NOT NULL
);


ALTER TABLE public.locks OWNER TO postgres;

--
-- TOC entry 200 (class 1259 OID 24671)
-- Name: person; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.person (
    id bigint NOT NULL,
    lastname text NOT NULL,
    firstname text NOT NULL,
    gender bigint,
    birthdate timestamp without time zone NOT NULL
);


ALTER TABLE public.person OWNER TO postgres;

--
-- TOC entry 203 (class 1259 OID 24740)
-- Name: student; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.student (
    grade bigint
)
INHERITS (public.person);


ALTER TABLE public.student OWNER TO postgres;

--
-- TOC entry 205 (class 1259 OID 24797)
-- Name: student_course; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.student_course (
    fk_course bigint NOT NULL,
    fk_student bigint NOT NULL
);


ALTER TABLE public.student_course OWNER TO postgres;

--
-- TOC entry 201 (class 1259 OID 24710)
-- Name: teacher; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.teacher (
    salary bigint,
    hiredate date
)
INHERITS (public.person);


ALTER TABLE public.teacher OWNER TO postgres;

--
-- TOC entry 2892 (class 2606 OID 24778)
-- Name: class class_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT class_pkey PRIMARY KEY (id);


--
-- TOC entry 2886 (class 2606 OID 24725)
-- Name: course course_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.course
    ADD CONSTRAINT course_pkey PRIMARY KEY (id);


--
-- TOC entry 2878 (class 2606 OID 24678)
-- Name: person person_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.person
    ADD CONSTRAINT person_pkey PRIMARY KEY (id);


--
-- TOC entry 2894 (class 2606 OID 24801)
-- Name: student_course student_course_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_course
    ADD CONSTRAINT student_course_pkey PRIMARY KEY (fk_course, fk_student);


--
-- TOC entry 2888 (class 2606 OID 24796)
-- Name: student student_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student
    ADD CONSTRAINT student_id_key UNIQUE (id);


--
-- TOC entry 2890 (class 2606 OID 24785)
-- Name: student student_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student
    ADD CONSTRAINT student_pkey PRIMARY KEY (id);


--
-- TOC entry 2882 (class 2606 OID 24794)
-- Name: teacher teacher_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.teacher
    ADD CONSTRAINT teacher_id_key UNIQUE (id);


--
-- TOC entry 2884 (class 2606 OID 24717)
-- Name: teacher teacher_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.teacher
    ADD CONSTRAINT teacher_pkey PRIMARY KEY (id);


--
-- TOC entry 2880 (class 2606 OID 24757)
-- Name: person unique id; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.person
    ADD CONSTRAINT "unique id" UNIQUE (id);


--
-- TOC entry 2895 (class 1259 OID 24815)
-- Name: ux_locks; Type: INDEX; Schema: public; Owner: postgres
--

CREATE UNIQUE INDEX ux_locks ON public.locks USING btree (jclass, jobject);


--
-- TOC entry 2898 (class 2606 OID 24802)
-- Name: student_course course fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_course
    ADD CONSTRAINT "course fk" FOREIGN KEY (fk_course) REFERENCES public.course(id);


--
-- TOC entry 2899 (class 2606 OID 24807)
-- Name: student_course student fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.student_course
    ADD CONSTRAINT "student fk" FOREIGN KEY (fk_student) REFERENCES public.student(id);


--
-- TOC entry 2896 (class 2606 OID 24726)
-- Name: course teaceer; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.course
    ADD CONSTRAINT teaceer FOREIGN KEY (fk_teacher) REFERENCES public.teacher(id);


--
-- TOC entry 2897 (class 2606 OID 24779)
-- Name: class teacherfk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.class
    ADD CONSTRAINT teacherfk FOREIGN KEY (fk_teacher) REFERENCES public.teacher(id);


-- Completed on 2022-01-16 16:42:09

--
-- PostgreSQL database dump complete
--


--
-- PostgreSQL database dump
--

-- Dumped from database version 16.2 (Debian 16.2-1.pgdg120+2)
-- Dumped by pg_dump version 16.2

-- Started on 2024-05-09 19:48:19

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

--
-- TOC entry 4 (class 2615 OID 2200)
-- Name: public; Type: SCHEMA; Schema: -; Owner: pg_database_owner
--

-- CREATE SCHEMA public;


ALTER SCHEMA public OWNER TO pg_database_owner;

--
-- TOC entry 3394 (class 0 OID 0)
-- Dependencies: 4
-- Name: SCHEMA public; Type: COMMENT; Schema: -; Owner: pg_database_owner
--

COMMENT ON SCHEMA public IS 'standard public schema';


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 215 (class 1259 OID 16385)
-- Name: Accounts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Accounts" (
    id text NOT NULL,
    "AccountNumber" bigint NOT NULL,
    "OwnerId" text NOT NULL,
    "Amount" numeric NOT NULL,
    "IsDeleted" boolean DEFAULT false NOT NULL,
    "OpenDate" timestamp with time zone
);


ALTER TABLE public."Accounts" OWNER TO postgres;

--
-- TOC entry 216 (class 1259 OID 16391)
-- Name: Customers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Customers" (
    id text NOT NULL,
    "Name" text NOT NULL,
    "IsDeleted" boolean DEFAULT false NOT NULL
);


ALTER TABLE public."Customers" OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16397)
-- Name: TransactionStatuses; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TransactionStatuses" (
    "Id" integer NOT NULL,
    "Name" text NOT NULL,
    "Description" text NOT NULL
);


ALTER TABLE public."TransactionStatuses" OWNER TO postgres;

--
-- TOC entry 218 (class 1259 OID 16402)
-- Name: TransactionStatuses_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public."TransactionStatuses" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."TransactionStatuses_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 219 (class 1259 OID 16403)
-- Name: Transactions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Transactions" (
    id text NOT NULL,
    "SenderAccountId" text NOT NULL,
    "SenderAccountAccountNumber" bigint NOT NULL,
    "RecipientAccountId" text NOT NULL,
    "RecipientAccountAccountNumber" bigint NOT NULL,
    "Amount" numeric NOT NULL,
    "TransactionStatusId" integer NOT NULL,
    "CreatedDate" timestamp with time zone DEFAULT '-infinity'::timestamp with time zone NOT NULL
);


ALTER TABLE public."Transactions" OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16408)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO postgres;

--
-- TOC entry 3383 (class 0 OID 16385)
-- Dependencies: 215
-- Data for Name: Accounts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Accounts" (id, "AccountNumber", "OwnerId", "Amount", "IsDeleted", "OpenDate") FROM stdin;
\.


--
-- TOC entry 3384 (class 0 OID 16391)
-- Dependencies: 216
-- Data for Name: Customers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Customers" (id, "Name", "IsDeleted") FROM stdin;
\.


--
-- TOC entry 3385 (class 0 OID 16397)
-- Dependencies: 217
-- Data for Name: TransactionStatuses; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."TransactionStatuses" ("Id", "Name", "Description") FROM stdin;
1	Created	Транзакция создана
2	Processing	Транзакция в обработке
3	Completed	Транзакция обработана
4	Cancelled	Транзакция отменена
\.


--
-- TOC entry 3387 (class 0 OID 16403)
-- Dependencies: 219
-- Data for Name: Transactions; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Transactions" (id, "SenderAccountId", "SenderAccountAccountNumber", "RecipientAccountId", "RecipientAccountAccountNumber", "Amount", "TransactionStatusId", "CreatedDate") FROM stdin;
\.


--
-- TOC entry 3388 (class 0 OID 16408)
-- Dependencies: 220
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20240424093425_Initial	8.0.2
20240505105044_isddeleted	8.0.2
20240509164726_adddates	8.0.2
\.


--
-- TOC entry 3395 (class 0 OID 0)
-- Dependencies: 218
-- Name: TransactionStatuses_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."TransactionStatuses_Id_seq"', 1, false);


--
-- TOC entry 3224 (class 2606 OID 16412)
-- Name: Accounts PK_Accounts; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Accounts"
    ADD CONSTRAINT "PK_Accounts" PRIMARY KEY (id, "AccountNumber");


--
-- TOC entry 3226 (class 2606 OID 16414)
-- Name: Customers PK_Customers; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Customers"
    ADD CONSTRAINT "PK_Customers" PRIMARY KEY (id);


--
-- TOC entry 3228 (class 2606 OID 16416)
-- Name: TransactionStatuses PK_TransactionStatuses; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TransactionStatuses"
    ADD CONSTRAINT "PK_TransactionStatuses" PRIMARY KEY ("Id");


--
-- TOC entry 3233 (class 2606 OID 16418)
-- Name: Transactions PK_Transactions; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "PK_Transactions" PRIMARY KEY (id);


--
-- TOC entry 3235 (class 2606 OID 16420)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 3222 (class 1259 OID 16421)
-- Name: IX_Accounts_OwnerId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Accounts_OwnerId" ON public."Accounts" USING btree ("OwnerId");


--
-- TOC entry 3229 (class 1259 OID 16422)
-- Name: IX_Transactions_RecipientAccountId_RecipientAccountAccountNumb~; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_RecipientAccountId_RecipientAccountAccountNumb~" ON public."Transactions" USING btree ("RecipientAccountId", "RecipientAccountAccountNumber");


--
-- TOC entry 3230 (class 1259 OID 16423)
-- Name: IX_Transactions_SenderAccountId_SenderAccountAccountNumber; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_SenderAccountId_SenderAccountAccountNumber" ON public."Transactions" USING btree ("SenderAccountId", "SenderAccountAccountNumber");


--
-- TOC entry 3231 (class 1259 OID 16424)
-- Name: IX_Transactions_TransactionStatusId; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX "IX_Transactions_TransactionStatusId" ON public."Transactions" USING btree ("TransactionStatusId");


--
-- TOC entry 3236 (class 2606 OID 16425)
-- Name: Accounts FK_Accounts_Customers_OwnerId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Accounts"
    ADD CONSTRAINT "FK_Accounts_Customers_OwnerId" FOREIGN KEY ("OwnerId") REFERENCES public."Customers"(id) ON DELETE SET NULL;


--
-- TOC entry 3237 (class 2606 OID 16430)
-- Name: Transactions FK_Transactions_Accounts_RecipientAccountId_RecipientAccountAc~; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "FK_Transactions_Accounts_RecipientAccountId_RecipientAccountAc~" FOREIGN KEY ("RecipientAccountId", "RecipientAccountAccountNumber") REFERENCES public."Accounts"(id, "AccountNumber") ON DELETE SET NULL;


--
-- TOC entry 3238 (class 2606 OID 16435)
-- Name: Transactions FK_Transactions_Accounts_SenderAccountId_SenderAccountAccountN~; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "FK_Transactions_Accounts_SenderAccountId_SenderAccountAccountN~" FOREIGN KEY ("SenderAccountId", "SenderAccountAccountNumber") REFERENCES public."Accounts"(id, "AccountNumber") ON DELETE SET NULL;


--
-- TOC entry 3239 (class 2606 OID 16440)
-- Name: Transactions FK_Transactions_TransactionStatuses_TransactionStatusId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Transactions"
    ADD CONSTRAINT "FK_Transactions_TransactionStatuses_TransactionStatusId" FOREIGN KEY ("TransactionStatusId") REFERENCES public."TransactionStatuses"("Id") ON DELETE CASCADE;


-- Completed on 2024-05-09 19:48:19

--
-- PostgreSQL database dump complete
--


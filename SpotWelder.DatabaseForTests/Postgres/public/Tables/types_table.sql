CREATE TABLE public.types_table (
	primary_key_int int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	decimal_col numeric(8, 2) NOT NULL,
	date_col date NOT NULL,
	datetime2_col timestamp(6) NOT NULL,
	ansi_string varchar(255) NOT NULL,
	unicode_string varchar(255) NOT NULL,
	CONSTRAINT types_table_pk PRIMARY KEY (primary_key_int)
);

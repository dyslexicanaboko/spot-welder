CREATE TABLE public.types_table_nullable (
	primary_key_int int4 GENERATED ALWAYS AS IDENTITY( INCREMENT BY 1 MINVALUE 1 MAXVALUE 2147483647 START 1 CACHE 1 NO CYCLE) NOT NULL,
	decimal_col_nullable numeric(8, 2) NULL,
	date_col_nullable date NULL,
	datetime2_col_nullable timestamp(6) NULL,
	ansi_string_nullable varchar(255) NULL,
	unicode_string_nullable varchar(255) NULL,
	CONSTRAINT types_table_nullable_pk PRIMARY KEY (primary_key_int)
);

alter table formfields add IsSummable bit not null default ((0));
go

update forms set NameArabic = NameEnglish where NameArabic is null or NameArabic =''
go

delete from formfields where id in (714, 715, 716)
go
Insert into formfields values ('11', NULL, N'48', N'table_start', N'الجدول بداية', N'Table start', N'17', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'22', N'0', N'0');
go
Insert into formfields values ('11', N'30', N'48', N'ScientificRank', N'الرتبة العلمية', N'Scientific Rank', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'13', N'0', N'0');
go
Insert into formfields values ('11', NULL, N'48', N'University', N'الجامعة', N'University', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'14', N'0', N'0');
go
Insert into formfields values ('11', N'31', N'48', N'college', N'الكلية', N'college', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'15', N'0', N'0');
go
Insert into formfields values ('11', N'32', N'48', N'scientificdepartment', N'القسم العلمي', N'scientific department', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'16', N'0', N'0');
go
Insert into formfields values ('11', NULL, N'48', N'phone\cellphone', N'الهاتف / الجوال', N'phone\cell phone', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'17', N'0', N'0');
go
Insert into formfields values ('11', NULL, N'48', N'Email', N'البريد الإلكتروني', N'Email', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'18', N'0', N'0');
go
Insert into formfields values ('11', NULL, N'48', N'ID', N'الهوية / الإقامة', N'ID', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'19', N'0', N'0');
go
Insert into formfields values ('11', N'33', N'48', N'Nationality', N'الجنسية', N'Nationality', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'20', N'0', N'0');
go
Insert into formfields values ('11', NULL, N'48', N'ProjectArea:GeneralSpecialization', N'مجال المشروع : التخصص العام', N'Project Area: General Specialization', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'21', N'0', N'0');
go
Insert into formfields values ('11', NULL, N'48', N'Specialization', N'التخصص الدقيق', N'Specialization', N'18', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'22', N'0', N'0');
go

Insert into formfields values ('11', NULL, N'48', N'table_end', N'نهاية الجدول', N'Table end', N'19', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'22', N'0', N'0');
go

Insert into formfields values ('11', NULL, N'48', N'name', N'الاسم', N'Name', N'1', N'1', N'0', N'1', NULL, NULL, NULL, NULL, NULL, NULL, N'22', N'0', N'0');
go

update formfields set [order] = 1 where id = 706;
go
update formfields set [order] = 2 where id = 707;
go
update formfields set [order] = 3 where id = 708;
go
update formfields set [order] = 4 where id = 709;
go
update formfields set [order] = 5 where id = 710;
go
update formfields set [order] = 6 where id = 711;
go
update formfields set [order] = 7 where id = 712;
go
update formfields set [order] = 8 where id = 713;
go
update formfields set [order] = 9 where id = 717;
go
update formfields set [order] = 10 where id = 718;
go
update formfields set [order] = 11 where id = 719;
go
update formfields set [order] = 12 where id = 720;
go
update formfields set [order] = 13 where id = 721;
go
update formfields set [order] = 14 where id = 722;
go
update formfields set [order] = 15 where id = 723;
go
update formfields set [order] = 16 where id = 724;
go
update formfields set [order] = 17 where id = 725;
go
update formfields set [order] = 18 where id = 726;
go
update formfields set [order] = 19 where id = 727;
go
update formfields set [order] = 20 where id = 728;
go
update formfields set [order] = 21 where id = 729;
go
update formfields set [order] = 22 where id = 730;
go
update formfields set [order] = 23 where id = 731;
go
update formfields set [order] = 24 where id = 732;
go
update formfields set [order] = 25 where id = 733;
go
update formfields set [order] = 26 where id = 924;
go
update formfields set [order] = 27 where id = 925;
go
update formfields set [order] = 28 where id = 926;
go
update formfields set [order] = 29 where id = 927;
go
update formfields set [order] = 30 where id = 928;
go
update formfields set [order] = 31 where id = 929;
go
update formfields set [order] = 32 where id = 930;
go
update formfields set [order] = 33 where id = 931;
go
update formfields set [order] = 34 where id = 932;
go
update formfields set [order] = 35 where id = 933;
go
update formfields set [order] = 36 where id = 934;
go
update formfields set [order] = 37 where id = 935;
go

update formfields set [order] = 38 where id = 728;
update formfields set [order] = 39 where id = 729;
update formfields set [order] = 40 where id = 730;
update formfields set [order] = 41 where id = 731;
update formfields set [order] = 42 where id = 732;
update formfields set [order] = 43 where id = 733;


go




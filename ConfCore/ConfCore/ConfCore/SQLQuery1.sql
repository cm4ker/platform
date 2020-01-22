create table [MDType]
(
	Id uniqueidentifier,
)

create table [MDProperty]
(
	Id uniqueidentifier,
)

Create table [Type]
(
	Id uniqueidentifier,
	ParentId uniqueidentifier,
	MDTypeID uniqueidentifier,
)

create table [Property]
(
	Id uniqueidentifier,
)
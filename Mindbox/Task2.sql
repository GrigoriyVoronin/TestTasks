-- Для точности выполнения задания хотелось бы видеть схему БД.
-- Делал из предположения, что БД состоит из следующих 3 таблиц:
-- Products (Id, Name), Categories(id, Name), ProductsCategories(ProductId, CategoryId);


select
    p."Name" as "Product Name",
    c."Name" as "Category Name"
from "Products" as p
left join "ProductsCategories" as pc
on p."Id" = pc."ProductId"
left join "Categories" as c
on c."Id" = pc."CategoryId";

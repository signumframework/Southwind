﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;
using System.Linq.Expressions;
using Signum.Utilities;

namespace Southwind.Entities
{
    [Serializable, EntityKind(EntityKind.Main, EntityData.Master)]
    public class ProductDN : Entity
    {
        [NotNullable, SqlDbType(Size = 40), UniqueIndex]
        string productName;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 40)]
        public string ProductName
        {
            get { return productName; }
            set { SetToStr(ref productName, value, () => ProductName); }
        }

        Lite<SupplierDN> supplier;
        [NotNullValidator]
        public Lite<SupplierDN> Supplier
        {
            get { return supplier; }
            set { Set(ref supplier, value, () => Supplier); }
        }

        Lite<CategoryDN> category;
        [NotNullValidator]
        public Lite<CategoryDN> Category
        {
            get { return category; }
            set { Set(ref category, value, () => Category); }
        }

        [NotNullable, SqlDbType(Size = 20)]
        string quantityPerUnit;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 20)]
        public string QuantityPerUnit
        {   
            get { return quantityPerUnit; }
            set { Set(ref quantityPerUnit, value, () => QuantityPerUnit); }
        }

        decimal unitPrice;
        [NumberIsValidator(ComparisonType.GreaterThan, 0), Unit("$")]
        public decimal UnitPrice
        {
            get { return unitPrice; }
            set
            {
                if (Set(ref unitPrice, value, () => UnitPrice))
                    Notify(() => ValueInStock);
            }
        }

        short unitsInStock;
        [NumberIsValidator(ComparisonType.GreaterThanOrEqual, 0)]
        public short UnitsInStock
        {
            get { return unitsInStock; }
            set
            {
                if (Set(ref unitsInStock, value, () => UnitsInStock))
                    Notify(() => ValueInStock);
            }
        }

        int reorderLevel;
        public int ReorderLevel
        {
            get { return reorderLevel; }
            set { Set(ref reorderLevel, value, () => ReorderLevel); }
        }

        bool discontinued;
        public bool Discontinued
        {
            get { return discontinued; }
            set { Set(ref discontinued, value, () => Discontinued); }
        }

        static Expression<Func<ProductDN, decimal>> ValueInStockExpression =
            p => p.unitPrice * p.unitsInStock;
        [Unit("$")]
        public decimal ValueInStock
        {
            get { return ValueInStockExpression.Evaluate(this); }
        }

        public override string ToString()
        {
            return productName;
        }
    }

    public enum ProductOperation
    {
        Save
    }

    [Serializable, EntityKind(EntityKind.Main, EntityData.Master)]
    public class SupplierDN : Entity
    {
        [NotNullable, SqlDbType(Size = 40), UniqueIndex]
        string companyName;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 40)]
        public string CompanyName
        {
            get { return companyName; }
            set { SetToStr(ref companyName, value, () => CompanyName); }
        }

        [SqlDbType(Size = 30)]
        string contactName;
        [StringLengthValidator(AllowNulls = true, Min = 3, Max = 30)]
        public string ContactName
        {
            get { return contactName; }
            set { Set(ref contactName, value, () => ContactName); }
        }

        [SqlDbType(Size = 30)]
        string contactTitle;
        [StringLengthValidator(AllowNulls = true, Min = 3, Max = 30)]
        public string ContactTitle
        {
            get { return contactTitle; }
            set { Set(ref contactTitle, value, () => ContactTitle); }
        }

        [NotNullable]
        AddressDN address;
        [NotNullValidator]
        public AddressDN Address
        {
            get { return address; }
            set { Set(ref address, value, () => Address); }
        }

        [NotNullable, SqlDbType(Size = 24)]
        string phone;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 24), TelephoneValidator]
        public string Phone
        {
            get { return phone; }
            set { Set(ref phone, value, () => Phone); }
        }

        [NotNullable, SqlDbType(Size = 24)]
        string fax;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 24), TelephoneValidator]
        public string Fax
        {
            get { return fax; }
            set { Set(ref fax, value, () => Fax); }
        }

        [SqlDbType(Size = int.MaxValue)]
        string homePage;
        [StringLengthValidator(AllowNulls = true, Min = 3, Max = int.MaxValue)]
        public string HomePage
        {
            get { return homePage; }
            set { Set(ref homePage, value, () => HomePage); }
        }

        public override string ToString()
        {
            return companyName;
        }
    }

    public enum SupplierOperation
    {
        Save
    }

    [Serializable, EntityKind(EntityKind.String, EntityData.Master)]
    public class CategoryDN : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string categoryName;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string CategoryName
        {
            get { return categoryName; }
            set { SetToStr(ref categoryName, value, () => CategoryName); }
        }

        [NotNullable, SqlDbType(Size = int.MaxValue)]
        string description;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = int.MaxValue)]
        public string Description
        {
            get { return description; }
            set { Set(ref description, value, () => Description); }
        }

        byte[] picture;
        public byte[] Picture
        {
            get { return picture; }
            set { Set(ref picture, value, () => Picture); }
        }

        static Expression<Func<CategoryDN, string>> ToStringExpression = e => e.CategoryName;
        public override string ToString()
        {
            return ToStringExpression.Evaluate(this);
        }
    }

    public enum CategoryOperation
    {
        Save
    }

    public enum ProductQuery
    {
        Current
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Signum.Entities;

namespace Southwind.Entities
{
    [Serializable]
    public class OrderDN : Entity
    {
        CustomerDN customer;
        [NotNullValidator]
        public CustomerDN Customer
        {
            get { return customer; }
            set { Set(ref customer, value, () => Customer); }
        }

        Lite<EmployeeDN> employee;
        [NotNullValidator]
        public Lite<EmployeeDN> Employee
        {
            get { return employee; }
            set { Set(ref employee, value, () => Employee); }
        }

        DateTime orderDate;
        public DateTime OrderDate
        {
            get { return orderDate; }
            set { Set(ref orderDate, value, () => OrderDate); }
        }

        DateTime requiredDate;
        public DateTime RequiredDate
        {
            get { return requiredDate; }
            set { Set(ref requiredDate, value, () => RequiredDate); }
        }

        DateTime? shippedDate;
        public DateTime? ShippedDate
        {
            get { return shippedDate; }
            set { Set(ref shippedDate, value, () => ShippedDate); }
        }

        Lite<ShipperDN> shipVia;
        public Lite<ShipperDN> ShipVia
        {
            get { return shipVia; }
            set { Set(ref shipVia, value, () => ShipVia); }
        }

        [SqlDbType(Size = 40)]
        string shipName;
        [StringLengthValidator(AllowNulls = true, Min = 3, Max = 40)]
        public string ShipName
        {
            get { return shipName; }
            set { Set(ref shipName, value, () => ShipName); }
        }

        AddressDN shipAddress;
        public AddressDN ShipAddress
        {
            get { return shipAddress; }
            set { Set(ref shipAddress, value, () => ShipAddress); }
        }

        MList<OrderDetailsDN> details;
        public MList<OrderDetailsDN> Details
        {
            get { return details; }
            set { Set(ref details, value, () => Details); }
        }
    }


    [Serializable]
    public class OrderDetailsDN : EmbeddedEntity
    {
        Lite<ProductDN> product;
        [NotNullValidator]
        public Lite<ProductDN> Product
        {
            get { return product; }
            set { Set(ref product, value, () => Product); }
        }

        decimal unitPrice;
        public decimal UnitPrice
        {
            get { return unitPrice; }
            set { Set(ref unitPrice, value, () => UnitPrice); }
        }

        int quantity;
        public int Quantity
        {
            get { return quantity; }
            set { Set(ref quantity, value, () => Quantity); }
        }

        decimal discount;
        public decimal Discount
        {
            get { return discount; }
            set { Set(ref discount, value, () => Discount); }
        }
    }

    [Serializable]
    public class ShipperDN : Entity
    {
        [NotNullable, SqlDbType(Size = 100), UniqueIndex]
        string companyName;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 100)]
        public string CompanyName
        {
            get { return companyName; }
            set { SetToStr(ref companyName, value, () => CompanyName); }
        }

        [NotNullable, SqlDbType(Size = 24)]
        string phone;
        [StringLengthValidator(AllowNulls = false, Min = 3, Max = 24), TelephoneValidator]
        public string Phone
        {
            get { return phone; }
            set { Set(ref phone, value, () => Phone); }
        }

        public override string ToString()
        {
            return companyName;
        }
    }
}

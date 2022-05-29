using AutoMapper;
using OrderImport.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderImport.DTOs;
public class DtosProfile : Profile
{
	public DtosProfile()
	{

		CreateMap<CustomerDto, Customer>().ForMember(d => d.Name, o => o.MapFrom(s => s.CustomerName));

		CreateMap<OrderDto, Order>();

	}
}

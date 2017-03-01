using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swashbuckle.AspNetCore.Swagger {
	public class FormDataOperationFilter : IOperationFilter {
		public void Apply(Operation operation, OperationFilterContext context) {
			var actattrs = context.ApiDescription.ActionAttributes();
			if (actattrs.OfType<HttpPostAttribute>().Any() ||
				actattrs.OfType<HttpPutAttribute>().Any())
				operation.Consumes = new[] { "multipart/form-data" };
		}
	}
}
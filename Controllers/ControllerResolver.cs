using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace ProductsApp.Controllers
{
	public class TypeCache
	{
		private Lazy<ConcurrentDictionary<Type, object>> _concDictionary = new Lazy<ConcurrentDictionary<Type, object>>(() => new ConcurrentDictionary<Type, object>());
		
		public object Add(Type type, object obj)
		{
			object value = this._concDictionary.Value.AddOrUpdate(type, obj, ((t, o) => o));

			return value;
		}

		public object Delete(Type type)
		{
			object value = null;
			this._concDictionary.Value.TryRemove(type, out value);

			return value;
		}

		public object Get(Type type)
		{
			object value = null;
			this._concDictionary.Value.TryGetValue(type, out value);

			return value;
		}
	}

	public class CustomControllerFactory : System.Web.Mvc.IControllerFactory
	{
		
	}

	public class CustomHttpControllerTypeResolver : DefaultHttpControllerTypeResolver
	{
		public CustomHttpControllerTypeResolver()
			: base(IsHttpEndpoint)
		{ }

		public static bool IsHttpEndpoint(Type type)
		{
			if (type == null) throw new ArgumentNullException("Type is null");

			return
				type.IsClass &&
				type.IsVisible &&
				!type.IsAbstract &&
				typeof(IHttpController).IsAssignableFrom(type);
		}
	}

	public class ByPassCacheSelector : DefaultHttpControllerSelector
	{
		public HttpConfiguration _configuration = null;
		public ByPassCacheSelector(HttpConfiguration configuration)
			: base(configuration)
		{
			this._configuration = configuration;
		}

		public override HttpControllerDescriptor SelectController(System.Net.Http.HttpRequestMessage request)
		{
			Assembly curAsm = Assembly.GetExecutingAssembly();
			var types = curAsm.GetTypes();

			string controllerName = base.GetControllerName(request);
			Type controllerType = (from t in types
								   where typeof(IHttpController).IsAssignableFrom(t) && t.Name.Equals(controllerName + "Controller", StringComparison.OrdinalIgnoreCase)
								   select t).SingleOrDefault();

			HttpControllerDescriptor descriptor = new HttpControllerDescriptor(this._configuration, controllerName, controllerType);

			return descriptor;
		}
	}
}
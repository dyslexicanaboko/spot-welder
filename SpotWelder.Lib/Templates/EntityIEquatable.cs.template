{{Namespaces}}

namespace {{Namespace}}
{
	public partial class {{ClassName}} 
		: IEquatable <{{ClassName}}>
	{
		public override bool Equals(object? obj) => this.Equals(obj as {{ClassName}});

		public bool Equals({{ClassName}}? other)
		{
			if (other is null) return false;

			if (object.ReferenceEquals(this, other)) return true;

			if (this.GetType() != other.GetType()) return false;

			return
{{PropertiesEquals}};
		}

		public override int GetHashCode() =>
{{PropertiesHashCode}};

		public static bool operator ==({{ClassName}}? lhs, {{ClassName}}? rhs)
		{
			if (lhs is not null) return lhs.Equals(rhs);

			return rhs is null;
		}

		public static bool operator !=({{ClassName}}? lhs, {{ClassName}}? rhs) => !(lhs == rhs);
	}
}
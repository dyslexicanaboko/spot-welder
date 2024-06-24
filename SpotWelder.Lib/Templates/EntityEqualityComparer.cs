{{Namespaces}}

namespace {{Namespace}}
{
	public class {{ClassName}}EqualityComparer
		: EqualityComparer<{{ClassName}}>
	{
		public override bool Equals({{ClassName}}? left, {{ClassName}}? right)
		{
			if (left == null && right == null) return true;

			if (left == null ^ right == null) return false;

			if (ReferenceEquals(left, right)) return true;

			if (left!.GetType() != right!.GetType()) return false;

			return
{{PropertiesEquals}};
		}

		public override int GetHashCode({{ClassName}}? obj)
		{
			if (obj == null) return -1;

			return
			{{PropertiesHashCode}};
		}
	}
}

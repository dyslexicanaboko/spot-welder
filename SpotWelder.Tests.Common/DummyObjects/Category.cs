namespace SpotWelder.Tests.Common.DummyObjects
{
  //Test object for a postgres database table
  //public.category
  public class Category
  {
    //category_id
    public int CategoryId { get; set; }

    //user_id
    public int UserId { get; set; }

    //name
    public string Name { get; set; }

    //created_on
    public DateTime CreatedOn { get; set; }

    //modified_on
    public DateTime ModifiedOn { get; set; }
  }
}

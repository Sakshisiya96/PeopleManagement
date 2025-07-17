namespace CrudTest
{
    public class MyMathUnitTesting
    {
        [Fact]
        public void MyMath_Null()
        {
            //Arrange
            MyMath mm = new MyMath();
            int a = 3;
            int b = 4;
            int result=mm.Add(a, b);
            //Act
            //Assert
            Assert.Equal(7, result);
        }
    }
}
#include "pch.h"
#include "SimpleMath.h"
#if __has_include("SimpleMath.g.cpp")
#include "SimpleMath.g.cpp"
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::WinUI3RCCPP::implementation
{
	double SimpleMath::add(double firstNumber, double secondNumber)
	{
		return firstNumber + secondNumber;
	}
	double SimpleMath::subtract(double firstNumber, double secondNumber)
	{
		return firstNumber - secondNumber;
	}
	double SimpleMath::multiply(double firstNumber, double secondNumber)
	{
		return firstNumber * secondNumber;
	}
	double SimpleMath::divide(double firstNumber, double secondNumber)
	{
		if (0 == secondNumber)
			return -1;
		return firstNumber / secondNumber;
	}
}

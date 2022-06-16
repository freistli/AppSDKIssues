#include "pch.h"
#include "MathPage.xaml.h"
#if __has_include("MathPage.g.cpp")
#include "MathPage.g.cpp"
#endif

using namespace winrt;
using namespace Microsoft::UI::Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace winrt::WinUI3RCCPP::implementation
{
    MathPage::MathPage()
    {
        InitializeComponent();
    }

    int32_t MathPage::MyProperty()
    {
        throw hresult_not_implemented();
    }

    void MathPage::MyProperty(int32_t /* value */)
    {
        throw hresult_not_implemented();
    }

    void MathPage::myButton_Click(IInspectable const&, RoutedEventArgs const&)
    {
         
        Result().Text(to_hstring( _wtof(ParamOne().Text().c_str()) + _wtof(ParamTwo().Text().c_str())));
    }
}

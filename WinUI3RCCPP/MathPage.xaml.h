#pragma once

#include "MathPage.g.h"

namespace winrt::WinUI3RCCPP::implementation
{
    struct MathPage : MathPageT<MathPage>
    {
        MathPage();

        int32_t MyProperty();
        void MyProperty(int32_t value);

        void myButton_Click(Windows::Foundation::IInspectable const& sender, Microsoft::UI::Xaml::RoutedEventArgs const& args);
    };
}

namespace winrt::WinUI3RCCPP::factory_implementation
{
    struct MathPage : MathPageT<MathPage, implementation::MathPage>
    {
    };
}

//
// Copyright © 2020 Terry Moreland
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

#pragma once

#include <optional>

namespace win32::credential_store 
{
    /// <summary>
    /// class storing one of two possible value types
    /// </summary>
    /// <typeparam name="TLeft">Primary value type allowing a wider range of construction</typeparam>
    /// <typeparam name="TRight">Seconary value type, typically a fall back or error value</typeparam>
    template <typename TLeft, typename TRight>
    class either final
    {
        static_assert(!std::is_same_v<TLeft, TRight>, "Types cannot be the same");
        std::optional<TLeft> m_left_value;
        std::optional<TRight> m_right_value;

    public:
        using value_type = TLeft;
        using left_value_type = TRight;
        using right_value_type = TRight;

        constexpr explicit either(TLeft&& left_value)
            : m_left_value(std::forward(left_value))
            , m_right_value(std::nullopt)
        {
        }
        template <class... TArgs,
            std::enable_if_t<std::is_constructible_v<TLeft, TArgs...>, int> = 0>
        constexpr explicit either(std::in_place_t, TArgs&&... arguments)
            : m_left_value(std::in_place, std::forward<TArgs>(arguments)...)
            , m_right_value(std::nullopt)
        {
        }
        template <class TElement, class... TArgs,
            std::enable_if_t<std::is_constructible_v<TLeft, std::initializer_list<TElement>&, TArgs...>, int> = 0>
        constexpr explicit either(std::in_place_t, std::initializer_list<TElement> list, TArgs&&... arguments)
            : m_left_value(std::in_place, list, std::forward<TArgs>(arguments)...)
            , m_right_value(std::nullopt)
        {
        }

        constexpr explicit either(TRight&& right_value)
            : m_left_value(std::nullopt)
            , m_right_value(right_value)
        {
        }
        constexpr explicit either(std::optional<TLeft>&& left_value)
            : m_left_value(std::move(left_value))
            , m_right_value(std::nullopt)
        {
        }
        constexpr explicit either(std::optional<TRight>&& right_value)
            : m_left_value(std::nullopt)
            , m_right_value(std::move(right_value))
        {
        }

        template <typename TValueType>
        [[nodiscard]] constexpr bool has_value() const
        {
            static_assert(std::is_same_v<TValueType, TLeft> || std::is_same_v<TValueType, TRight>, "invalid type");

            if constexpr (std::is_same_v<TValueType, TLeft>) {
                return m_left_value.has_value();
            } else {
                return m_right_value.has_value();
            }

        }

        template <typename TValueType>
        [[nodiscard]] constexpr TValueType& value()
        {
            static_assert(std::is_same_v<TValueType, TLeft> || std::is_same_v<TValueType, TRight>, "invalid type");

            if constexpr (std::is_same_v<TValueType, TLeft>) {
                return m_left_value.value();
            } else {
                return m_right_value.value();
            }
        }

        template <typename TValueType>
        [[nodiscard]] constexpr TValueType const& value() const
        {
            static_assert(std::is_same_v<TValueType, TLeft> || std::is_same_v<TValueType, TRight>, "invalid type");

            if constexpr (std::is_same_v<TValueType, TLeft>) {
                return m_left_value.value();
            } else {
                return m_right_value.value();
            }
        }

        template <typename TValueType>
        [[nodiscard]] constexpr TValueType& value_or(TValueType& else_value)
        {
            static_assert(std::is_same_v<TValueType, TLeft> || std::is_same_v<TValueType, TRight>, "invalid type");
            return m_left_value.value_or(else_value);
        }
        template <typename TValueType>
        [[nodiscard]] constexpr TValueType const& value_or(TValueType const& else_value) const
        {
            static_assert(std::is_same_v<TValueType, TLeft> || std::is_same_v<TValueType, TRight>, "invalid type");
            if constexpr (std::is_same_v<TValueType, TLeft>) {
                return m_left_value.value_or(else_value);
            } else {
                return m_right_value.value_or(else_value);
            }
        }
        template <typename TValueType, typename TSupplier>
        [[nodiscard]] constexpr TValueType& value_or(TSupplier const& supplier)
        {
            if (has_value<TValueType>()) 
                return value<TValueType>();
            return supplier();
        }
        template <typename TValueType, typename TSupplier>
        [[nodiscard]] constexpr TValueType const& value_or(TSupplier const& supplier) const
        {
            if (has_value<TValueType>()) 
                return value<TValueType>();
            return supplier();
        }
        /*
         * TODO: fix the template issues prventing this from specializing
        template <typename TValueType, typename TSupplier,
            std::enable_if_t<std::is_trivially_copy_assignable_v<TLeft>, int> = 0>
        [[nodiscard]] constexpr TValueType value_or(TSupplier const& supplier)
        {
            if (has_value<TValueType>()) 
                return value<TValueType>();
            return supplier();
        }
        */

        [[nodiscard]] constexpr explicit operator TLeft&() 
        {
            return m_left_value.value();
        }
        [[nodiscard]] constexpr explicit operator TLeft const&() const
        {
            return m_left_value.value();
        }

        constexpr explicit operator TRight&() 
        {
            return m_right_value.value();
        }
        constexpr explicit operator TRight const&() const
        {
            return m_right_value.value();
        }
        constexpr TLeft& right_value_or(TLeft& else_value)
        {
            return m_right_value.value_or(else_value);
        }
    };

    template <typename TLeft, typename TRight>
    constexpr either<TLeft, TRight> make_left(TLeft&& left_value)
    {
        return either<TLeft, TRight>(left_value);
    }
    template <typename TLeft, typename TRight, class... TArgs,
        std::enable_if_t<std::is_constructible_v<TLeft, TArgs...>, int> = 0>
    constexpr either<TLeft, TRight> make_left(std::in_place_t, TArgs&&... arguments)
    {
        return either<TLeft, TRight>(std::in_place, std::forward<TArgs>(arguments)...);
    }
    template <typename TLeft, typename TRight, class TElement, class... TArgs,
        std::enable_if_t<std::is_constructible_v<TLeft, std::initializer_list<TElement>&, TArgs...>, int> = 0>
    constexpr either<TLeft, TRight> make_left(std::in_place_t, std::initializer_list<TElement> list, TArgs&&... arguments)
    {
        return either<TLeft, TRight>(std::in_place, list, std::forward<TArgs>(arguments)...);
    }

    template <typename TLeft, typename TRight>
    constexpr either<TLeft, TRight> make_right(TRight&& right_value)
    {
        return either<TLeft, TRight>(right_value);
    }


}

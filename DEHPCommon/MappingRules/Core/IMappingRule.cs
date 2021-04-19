// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMappingRule.cs"company="RHEA System S.A.">
//    Copyright(c) 2020 RHEA System S.A.
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Kamil Wojnowski, Nathanael Smiechowski.
// 
//    This file is part of DEHP Common Library
// 
//    The DEHPCommon is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The DEHPCommon is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Lesser General Public License
//    along with this program; if not, write to the Free Software Foundation,
//    Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace DEHPCommon.MappingRules.Core
{
    using System;

    /// <summary>
    /// Defines a mapping rule for <see cref="MappingRule{T,T}"/>
    /// </summary>
    /// <typeparam name="TInput">The input <see cref="Type"/></typeparam>
    /// <typeparam name="TOutput">The output <see cref="Type"/></typeparam>
    internal interface IMappingRule<in TInput, out TOutput> : IMappingRule
    {
        /// <summary>
        /// Transforms a object of type <see cref="TInput"/> to another one of type <see cref="TOutput"/>
        /// </summary>
        TOutput Transform(TInput input);
    }

    /// <summary>
    /// Definition of the <see cref="IMappingRule"/> interface
    /// </summary>
    public interface IMappingRule
    {
    }
}

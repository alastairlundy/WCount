/*
    BasisBox - WCount Library
    Copyright (C) 2024 Alastair Lundy

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, version 3 of the License.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Threading.Tasks;

namespace WCountLib.Abstractions
{
    public interface IWordCounter
    {
        public Task<ulong> CountWordsAsync(string s);
        public ulong CountWords(string s);

        public Task<ulong> CountWordsInFileAsync(string filePath);
        public ulong CountWordsInFile(string filePath);

        public Task<ulong> CountWordsAsync(IEnumerable<string> enumerable);
        public ulong CountWords(IEnumerable<string> enumerable);
    }
}
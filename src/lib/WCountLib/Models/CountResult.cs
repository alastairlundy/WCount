/*
    WCountLib
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
*/

namespace WCountLib.Models;

/// <summary>
/// The result of a counting operation.
/// </summary>
/// <remarks>
/// Requested counts carry their counted value, which is always greater than or equal
/// to zero. Unrequested counts carry <c>-1</c>; consumers treat <c>-1</c> as
/// not-requested and consult the <see cref="CountRequest"/> as the primary validity rule.
/// </remarks>
public sealed record CountResult(long Words, long Lines, long Bytes, long Characters, long MaximumLineLength);

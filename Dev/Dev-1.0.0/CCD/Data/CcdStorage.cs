// CCD Consolidation and De-duplication Engine
// Copyright (C) 2013  CreateIT Healthcare Solutions, Inc.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see {http://www.gnu.org/licenses/}.

// Contact Info:
//     Jonathan Meade
//     CreateIT Healthcare Solutions, Inc.
//     516 SW 15th Street
//     Richmond, IN 47374
//     jmeade@createit-inc.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Data
{
    public class MergeJob
    {
        public Guid MergeId { get; set; }
        public XDocument Ccd { get; set; }
    }

    public sealed class CcdStorage
    {
        private List<MergeJob> _mergeJobs;

        public List<MergeJob> MergeJobs
        {
            get { return _mergeJobs; }
            set { _mergeJobs = value; }
        }

        private CcdStorage()
        {
            _mergeJobs = new List<MergeJob>();
        }

        private static CcdStorage _objCcdStorage = new CcdStorage();
        private static readonly object Padlock = new object();

        public static CcdStorage ObjCcdStorage
        {
            get
            {
                lock (Padlock)
                {
                    return _objCcdStorage ?? (_objCcdStorage = new CcdStorage());
                }
            }
         }

    }
}

﻿using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Bulldog3
{
    public class Bulldog3Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Bulldog3";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "Buldog plug-in: General purpose plug-in developed by Roberto Monesi @ SCA";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("53cdb89c-8bd6-4023-b937-c594e39e9332");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Roberto Monesi";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "roberto.monesi@outlook.com";
            }
        }
    }
}

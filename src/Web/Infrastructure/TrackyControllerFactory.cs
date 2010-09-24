﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ninject;

namespace Web.Infrastructure
{
    public class TrackyControllerFactory : DefaultControllerFactory
    {
        private IKernel _kernel = new StandardKernel(new TrackyServices());

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }

            return _kernel.Get(controllerType) as IController;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using ChinookSystem.BLL;
using ChinookSystem.ViewModels;
#endregion


namespace WebApp.Pages
{
    public class ListtoListModel : PageModel
    {

        #region Constructor for services and DI variables
        private readonly AboutServices _aboutservices;
        public ListtoListModel(AboutServices aboutservices)
        {
            _aboutservices = aboutservices;
        }

        [TempData]
        public string FeedBack { get; set; }

        public string ErrorMessage { get; set; }
        #endregion

        //will contain the original colors from AboutServices
        [BindProperty]
        public List<NamedColor> AvailableColors { get; set; }

        //will contain the selected colors by the user
        //the new() is need because the variable is used
        //  in generating the table on the 1st generation of the page
        //You can loop thru an empty list BUT not a null variable
        [BindProperty]
        public List<NamedColor> ColorPallete { get; set; } = new();

        //will hold the selected color ( RgbCode )
        [BindProperty]
        public string SelectedColor { get; set; }

        //will be the warmth list for select
        //[BindProperty]
        public List<SelectionList> Warmth { get; set; }

        public void OnGet()
        {
            AvailableColors = _aboutservices.ListHMTLColors();
            Warmth = _aboutservices.ColorWarmth();
        }
        public IActionResult OnPostAddItem()
        {
            // SelectedColor holds the value of the pressed button
            var found = AvailableColors.SingleOrDefault(x => x.Name == SelectedColor);
            // If the color is on the AvailableColors
            if (found != null)
            {
                //Remove from AvailableColors list
                AvailableColors.Remove(found);
                //Add to ColorPallete list
                ColorPallete.Add(found);
            }
            //needed to regenerate select control
            Warmth = _aboutservices.ColorWarmth();
            //default is remain on page if returndatatype is void instead of IActionResult
            //use the data within the lists
            return Page();
        }

        public IActionResult OnPostRemoveItem()
        {
            var found = ColorPallete.SingleOrDefault(x => x.Name == SelectedColor);
            if (found != null)
            {
                AvailableColors.Add(found);
                ColorPallete.Remove(found);
            }
            Warmth = _aboutservices.ColorWarmth();
            return Page();
        }
    }
}

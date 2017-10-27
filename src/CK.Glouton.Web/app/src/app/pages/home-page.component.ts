import { Component } from '@angular/core';

@Component({
  selector: 'home',
  template: `
    <h3>Home Page</h3>
    <div>
      This is our home page!
    </div>
    <div>
      <dateRangePicker></dateRangePicker>
    </div>
  `
})
export class HomePageComponent {
}

# Action Maps

## Inspiration
For some of our team it was our first time in the United States. We were staying at a place in downtown LA on the 49th floor. You could see everything far off to the horizon: Planes, cars in small streets and on the highway, trains, a lot of police, firefighters, bicycles - a dense, rich big city life. It reminded us of earlier ego-simulation sandboxes like SimCity. There's just so much information to gather from such a vantage point, two-dimensional maps just can't convey the richness and relationships of it.

Jeff's background is in connected devices and big data, and together with Mille's conceptual and visionary skills we knew we wanted to explore the intersection of creativity, data, and VR.

## What it does
We display real time data, acquired from freely accessible data sources, plotted in 3d (x,y,z) on a 3d map of Los Angeles using ESRI’s ArcGIS Framework, and play a live audio feed of tweets from around LA .

## How we built it
ESRI provides a framework for combining GIS data (a common interchange format for geospatial information) and presenting it in 3d. For the hackathon, they made available to us a beta of their SDK for AR/VR, which featured stereoscopic rendering. To this framework, we brought in and plotted information from several sources, including government GIS servers, amateur radio, and the public transit and bikesharing system

## Challenges we ran into
* Our flight to LA was delayed four hours

* Jeff's laptop's motherboard fried an hour into the hackathon (but we were able to use an excellent loaner PC from HP! Thanks!)

* Twitter provides location information in their tweets... about one out of every thousand. Rather than plotting them, we decided to use them as the audio feed of information

* We targeted originally the Windows Mixed Reality headsets, however the ESRI framework didn't support them in the end. We changed targets to Android as it's what we had available and what was supported

* The android handset we have heats up very quickly and starts to throttle

* We had problems getting to our hotel because Donald Trump was in LA

* Lots of yak-shaving around pre-requisites to getting development going

## Accomplishments that we're proud of
* Learning new frameworks with enough time to make it do something (Xamarin, ESRI's SDK)
* We were able to realize something approaching our original lofty goals
* Teamwork was problem-free
* We met really cool people working on amazing projects

## What we learned

We plan the development journey pretty well
20% ideation
20% research tools and general doability
20% content production
40% debugging and polishing

We learned how to use Github's project management

* It's possible to complete a project while still getting sleep--which is critical for when problems come up late

* Good git hygiene saved the day when the computer broke... we only lost about an hour (including computer setup) 

* The mentors are super helpful!

* LA actually has a massive public transit system, despite its reputation to the contrary

## What's next for Action Maps
* More data sources!
* Experimentation with more ways of conveying information, colors, sounds, spatial relationships, motion--there's so much to explore
* Polish the LA Action Map and investigate if this idea provides a new approach for visualization of complex data in general, while not only doing VR, but AR as well.
*  VR for an immersive solo experience - the hovering omniscient panopticon
* AR for collaborative work - the miniature toy town.
* Realtime collaboration on real-time data?

## Special Thanks
Jaxon: For letting us borrow your Google Cardboard!
HP: For loaning us an awesome backpack PC and MR headsets
ESRI: For making a really well documented Framework and going above-and-beyond supporting us
Hackathon Organizers: Great location, great food, great people!

## NOTE!
There is stuff missing. Building data you'll need to get from LA yourself. Twitter API credentials you'll need to fill in, and you need to ask ESRI for access to their SDK.
For android, put the building data LARIAC_BUILDINGS_2014.slpk to /mnt/sdcard with adb

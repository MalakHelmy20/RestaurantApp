const unsplash = (id: string) =>
  `https://images.unsplash.com/${id}?auto=format&fit=crop&w=800&h=800&q=80`

const commons = (file: string, width = 800) =>
  `https://commons.wikimedia.org/wiki/Special:FilePath/${encodeURIComponent(file)}?width=${width}`

/** Unsplash photos visually audited against the dish they represent. */
const U = {
  bbqBurger: unsplash('photo-1553979459-d2229ba7433b'),
  doubleBurger: unsplash('photo-1572802419224-296b0aeee0d9'),
  mushroomBurger: unsplash('photo-1771875600020-2a77a84c27b1'),
  burgerFries: unsplash('photo-1551782450-17144efb9c50'),
  smashBurger: unsplash('photo-1594212699903-ec8a3eca50f5'),
  crispyChicken: unsplash('photo-1606755962773-d324e0a13086'),
  fries: unsplash('photo-1576107232684-1279f390859f'),
  loadedFries: unsplash('photo-1573080496219-bb080dd4f877'),
  onionRings: unsplash('photo-1639024471283-03518883512d'),
  milkshake: unsplash('photo-1572490122747-3968b75cc699'),
  nuggets: unsplash('photo-1562967914-608f82629710'),
  soda: unsplash('photo-1622483767028-3f66f32aef97'),
  wrap: unsplash('photo-1626700051175-6818013e1d4f'),
  margherita: unsplash('photo-1574071318508-1cdbab80d002'),
  pizza: unsplash('photo-1513104890138-7c749659a591'),
  lasagna: unsplash('photo-1574894709920-11b28e7367e3'),
  carbonara: unsplash('photo-1612874742237-6526221588e3'),
  pasta: unsplash('photo-1555949258-eb67b1ef0ceb'),
  shrimpPasta: unsplash('photo-1563379926898-05f4575a45d8'),
  bruschetta: unsplash('photo-1572695157366-5e585ab2b69f'),
  caprese: unsplash('photo-1592417817098-8fd3d9eb14a5'),
  tiramisu: unsplash('photo-1571877227200-a0d98ea607e9'),
  sushiRolls: unsplash('photo-1579871494447-9811cf80d66c'),
  sushiPlatter: unsplash('photo-1611143669185-af224c5e3252'),
  salmonSushi: unsplash('photo-1583623025817-d180a2221d0a'),
  miso: unsplash('photo-1547592166-23ac45744acd'),
  teriyaki: unsplash('photo-1588166524941-3bf61a9c41db'),
  grilledFish: unsplash('photo-1615141982883-c7ad0e69fd62'),
  salmonPlate: unsplash('photo-1467003909585-2f8a72700288'),
  prawns: unsplash('photo-1559737558-2f5a35f4523b'),
  mixedGrill: unsplash('photo-1555939594-58d7cb561ad1'),
  salad: unsplash('photo-1512621776951-a57141f2eefd'),
  chickenSalad: unsplash('photo-1546793665-c74683f339c1'),
  grainBowl: unsplash('photo-1546069901-ba9599a7e63c'),
  veggieBowl: unsplash('photo-1540420773420-3366772f4999'),
  avocadoToast: unsplash('photo-1482049016688-2d3e1b311543'),
  smoothie: unsplash('photo-1623428187969-5da2dcea5ebf'),
  chia: unsplash('photo-1488477181946-6428a0291777'),
  lobsterBisque: unsplash('photo-1534939561126-855b8675edd7'),
  seafoodSoup: unsplash('photo-1617093727343-374698b1b08d'),
  dining: unsplash('photo-1414235077428-338989a2e8c0'),
  restaurant: unsplash('photo-1517248135467-4c7edcad34c4'),
  plated: unsplash('photo-1504674900247-0877df9cc836'),
} as const

/** Wikimedia photos used when the exact dish must be identifiable. */
const W = {
  koshari: commons('Egyptian_food_Koshary.jpg'),
  fulMedames: commons('Ful_medames_(arabic_meal).jpg'),
  hawawshi: commons('Egyptian_meatloaf.jpg'),
  molokhia: commons('Molokheya_Egypt,_2012.JPG'),
  omAli: commons('Umm_Ali.JPG'),
  babaGhanoush: commons('Baba_Ganoush_05of05_(8735238183).jpg'),
  fatteh: commons('Egyptian_Fattah.jpg'),
  grapeLeaves: commons('Yaprak_sarma,_Kayseri_style.jpg'),
  kofta: commons('Kabab_koobideh_bbq_persian_food.jpg'),
  classicBurger: commons('RedDot_Burger.jpg'),
  cheeseBurger: commons('Cheeseburger.jpg'),
  chickenBurger: commons('Fried_chicken_sandwich_with_comeback_sauce.jpg'),
  friedCalamari: commons('Calamares_a_la_romana.jpg'),
  chickenParm: commons('Chicken_parm_at_a_diner.jpg'),
  padThai: commons('Phat_Thai_kung_Chang_Khien_street_stall.jpg'),
  fishAndChips: commons('Fish_and_chips_blackpool.jpg'),
  pepperoni: commons('Pepperoni_Pizza_(29204589095).jpg'),
  alfredo: commons('Fettuccine_Alfredo_originals.jpg'),
  californiaRoll: commons('California_Sushi_(26571101885).jpg'),
  dragonRoll: commons('Golden_Maki_Vegetarian_Dragon_sushi_roll.jpg'),
  ramen: commons('Shoyu_Ramen（Tokyo_Ramen）_-_01.jpg'),
  gyoza: commons('Japanese_pan_fried_gyoza.jpg'),
  kungPao: commons('Kung-pao-shanghai.jpg'),
  springRolls: commons('Spring_Rolls_(3357696061).jpg'),
  falafel: commons('Falafels_2.jpg'),
  falafelWrap: commons('Falafel_in_a_pita.jpg'),
  hotDog: commons('Hot_dog_with_mustard.png'),
  cheesyFries: commons('Shake_shack_cheese_fries.jpg'),
  coleslaw: commons('2015-12-20_Spitzkohlsalat_mit_Möhren_anagoria.JPG'),
  shrimpSkewers: commons('Grilled_Shrimp_Spiedini_macaronigrill_(10253672423).jpg'),
  octopus: commons('2024-02-10_Octopus_at_Restaurant_Uri_Buri_in_Acre_anagoria.jpg'),
  grilledWrap: commons('Chicken_wrap_2.jpg'),
  mushroomRisotto: commons('Risotto_ai_funghi_porcini.JPG'),
  miso: commons('Miso_Soup_001.jpg'),
} as const

function normalize(value: string) {
  return value
    .toLowerCase()
    .normalize('NFKD')
    .replace(/[^a-z0-9]+/g, ' ')
    .trim()
}

/**
 * Exact menu names in this project. Same visual product may share a photo
 * across restaurants; variants must not.
 */
const EXACT_DISHES: Record<string, string> = {
  'avocado toast': U.avocadoToast,
  'baba ghanoush': W.babaGhanoush,
  'bbq bacon burger': U.bbqBurger,
  'beef ramen': W.ramen,
  'bruschetta al pomodoro': U.bruschetta,
  'california roll': W.californiaRoll,
  'caprese salad': U.caprese,
  cheeseburger: W.cheeseBurger,
  'cheesy fries': W.cheesyFries,
  'chia pudding': U.chia,
  'chicken burger': W.chickenBurger,
  'chicken parmesan': W.chickenParm,
  'chicken teriyaki': U.teriyaki,
  'chicken wrap': U.wrap,
  'classic burger': W.classicBurger,
  coleslaw: W.coleslaw,
  'crispy chicken sandwich': U.crispyChicken,
  'double smash burger': U.doubleBurger,
  'dragon roll': W.dragonRoll,
  'falafel wrap': W.falafelWrap,
  fatteh: W.fatteh,
  'fish and chips': W.fishAndChips,
  'french fries': U.fries,
  'fried calamari': W.friedCalamari,
  'ful medames': W.fulMedames,
  'green smoothie': U.smoothie,
  'grilled chicken salad': U.chickenSalad,
  'grilled chicken wrap': W.grilledWrap,
  'grilled kofta': W.kofta,
  'grilled octopus': W.octopus,
  'grilled salmon bowl': U.salmonPlate,
  'grilled sea bass': U.grilledFish,
  'grilled shrimp skewers': W.shrimpSkewers,
  'gyoza dumplings': W.gyoza,
  hawawshi: W.hawawshi,
  'hot dog': W.hotDog,
  koshari: W.koshari,
  koshary: W.koshari,
  kushari: W.koshari,
  'kung pao chicken': W.kungPao,
  'lasagna al forno': U.lasagna,
  'loaded fries': U.loadedFries,
  'lobster bisque': U.lobsterBisque,
  'margherita pizza': U.margherita,
  milkshake: U.milkshake,
  'miso soup': W.miso,
  'mixed grill platter': U.mixedGrill,
  'mixed seafood grill': U.prawns,
  'molokhia with rabbit': W.molokhia,
  molokhia: W.molokhia,
  'mushroom swiss burger': U.mushroomBurger,
  'nuggets combo': U.nuggets,
  'om ali': W.omAli,
  'onion rings': U.onionRings,
  'pad thai': W.padThai,
  'pasta alfredo': W.alfredo,
  'pepperoni pizza': W.pepperoni,
  'quinoa bowl': U.grainBowl,
  'risotto ai funghi': W.mushroomRisotto,
  'salmon sushi set': U.salmonSushi,
  'seafood soup': U.seafoodSoup,
  'shrimp pasta': U.shrimpPasta,
  'soft drink': U.soda,
  'spaghetti carbonara': U.carbonara,
  'spring rolls': W.springRolls,
  'stuffed grape leaves': W.grapeLeaves,
  tiramisu: U.tiramisu,
  'veggie bowl': U.veggieBowl,
}

type Rule = { test: RegExp; image: string }

/** Most specific patterns first. Used for newly created dishes. */
const DISH_RULES: Rule[] = [
  { test: /\bkoshari|\bkoshary|\bkushari/, image: W.koshari },
  { test: /\bful medames|\bful mudammas|\bfoul medames|\bful\b.*fava/, image: W.fulMedames },
  { test: /\bhawawshi|\bhawwaoshi/, image: W.hawawshi },
  { test: /\bmolokhia|\bmulukhiyah|\bmolokheya/, image: W.molokhia },
  { test: /\bom ali|\bumm ali|\bomali/, image: W.omAli },
  { test: /\bbaba ghanoush|\bbaba ganoush|\bbaba ghanouj/, image: W.babaGhanoush },
  { test: /\bfatteh|\bfattah|\bfatta\b/, image: W.fatteh },
  { test: /\bgrape leaves|\bwaraq|\bdolma|\bsarma/, image: W.grapeLeaves },
  { test: /\bkofta|\bkoobideh|\bkefte/, image: W.kofta },
  { test: /\bfalafel wrap|\bfalafel pita/, image: W.falafelWrap },
  { test: /\bfalafel|\btaameya|\btaamiya/, image: W.falafel },
  { test: /\bshawarma|\bshwarma/, image: U.mixedGrill },
  { test: /\bmushroom swiss|\bmushroom burger/, image: U.mushroomBurger },
  { test: /\bbbq bacon|\bbacon burger|\bbbq burger/, image: U.bbqBurger },
  { test: /\bdouble smash|\bsmash burger|\bdouble burger/, image: U.doubleBurger },
  { test: /\bchicken burger|\bchicken patty/, image: W.chickenBurger },
  { test: /\bcrispy chicken sandwich|\bfried chicken sandwich/, image: U.crispyChicken },
  { test: /\bcheese burger|\bcheeseburger/, image: W.cheeseBurger },
  { test: /\bclassic burger|\bhamburger\b/, image: W.classicBurger },
  { test: /\bpepperoni/, image: W.pepperoni },
  { test: /\bmargherita/, image: U.margherita },
  { test: /\bdragon roll/, image: W.dragonRoll },
  { test: /\bcalifornia roll/, image: W.californiaRoll },
  { test: /\bsalmon sushi|\bnigiri|\bsashimi/, image: U.salmonSushi },
  { test: /\bpad thai|\bphat thai/, image: W.padThai },
  { test: /\bramen/, image: W.ramen },
  { test: /\bgyoza|\bdumpling/, image: W.gyoza },
  { test: /\bmiso/, image: W.miso },
  { test: /\bkung pao|\bgong bao/, image: W.kungPao },
  { test: /\bteriyaki/, image: U.teriyaki },
  { test: /\bspring roll/, image: W.springRolls },
  { test: /\bcarbonara/, image: U.carbonara },
  { test: /\balfredo/, image: W.alfredo },
  { test: /\blasagna|\blasagne/, image: U.lasagna },
  { test: /\brisotto/, image: W.mushroomRisotto },
  { test: /\bbruschetta/, image: U.bruschetta },
  { test: /\bcaprese/, image: U.caprese },
  { test: /\btiramisu/, image: U.tiramisu },
  { test: /\bchicken parmesan|\bchicken parm|\bparmigiana/, image: W.chickenParm },
  { test: /\bshrimp pasta|\bprawn pasta|\bseafood pasta/, image: U.shrimpPasta },
  { test: /\bfried calamari|\bcalamari|\bfried squid/, image: W.friedCalamari },
  { test: /\boctopus|\bpulpo/, image: W.octopus },
  { test: /\bsea bass|\bseabass|\bgrilled fish/, image: U.grilledFish },
  { test: /\bshrimp skewer|\bprawn skewer|\bgrilled shrimp/, image: W.shrimpSkewers },
  { test: /\blobster bisque/, image: U.lobsterBisque },
  { test: /\bfish and chips|\bfish & chips/, image: W.fishAndChips },
  { test: /\bseafood soup|\bchowder|\bcioppino/, image: U.seafoodSoup },
  { test: /\bmixed seafood|\bseafood grill/, image: U.prawns },
  { test: /\bmixed grill/, image: U.mixedGrill },
  { test: /\bcheesy fries|\bcheese fries/, image: W.cheesyFries },
  { test: /\bloaded fries/, image: U.loadedFries },
  { test: /\bonion ring/, image: U.onionRings },
  { test: /\bfrench fries|\bfries\b/, image: U.fries },
  { test: /\bmilkshake|\bshake\b/, image: U.milkshake },
  { test: /\bhot dog|\bhotdog/, image: W.hotDog },
  { test: /\bnugget/, image: U.nuggets },
  { test: /\bsoft drink|\bsoda|\bcola|\bpepsi|\bcoke/, image: U.soda },
  { test: /\bcoleslaw|\bcole slaw/, image: W.coleslaw },
  { test: /\bgrilled chicken wrap/, image: W.grilledWrap },
  { test: /\bchicken wrap/, image: U.wrap },
  { test: /\bavocado toast/, image: U.avocadoToast },
  { test: /\bchia/, image: U.chia },
  { test: /\bgreen smoothie|\bsmoothie/, image: U.smoothie },
  { test: /\bquinoa/, image: U.grainBowl },
  { test: /\bgrilled chicken salad|\bchicken salad/, image: U.chickenSalad },
  { test: /\bsalmon bowl|\bgrilled salmon/, image: U.salmonPlate },
  { test: /\bveggie bowl|\bvegetable bowl|\bgreen bowl/, image: U.veggieBowl },
  { test: /\bpizza/, image: U.pizza },
  { test: /\bburger/, image: W.classicBurger },
  { test: /\bsushi|\bmaki|\broll\b/, image: U.sushiRolls },
  { test: /\bpasta|\bspaghetti|\bfettuccine/, image: U.pasta },
  { test: /\bsalad/, image: U.salad },
  { test: /\bwrap/, image: U.wrap },
]

const CATEGORY_IMAGES: Rule[] = [
  { test: /\begyptian/, image: W.koshari },
  { test: /\bmiddle eastern|\bmezze|\blevant/, image: W.babaGhanoush },
  { test: /\bpizza/, image: U.pizza },
  { test: /\bburger/, image: W.classicBurger },
  { test: /\bsushi/, image: U.sushiRolls },
  { test: /\bitalian|\bpasta/, image: U.pasta },
  { test: /\bseafood/, image: U.grilledFish },
  { test: /\basian/, image: W.padThai },
  { test: /\bhealthy|\bvegetarian|\bvegan/, image: U.salad },
  { test: /\bfast food|\bamerican/, image: U.fries },
]

type Cuisine =
  | 'egyptian'
  | 'seafood'
  | 'burger'
  | 'italian'
  | 'sushi'
  | 'healthy'
  | 'fastfood'
  | 'dining'

const CUISINE_PHOTOS: Record<Cuisine, readonly string[]> = {
  egyptian: [W.koshari, U.mixedGrill, W.babaGhanoush, W.falafel],
  seafood: [U.grilledFish, U.salmonPlate, U.shrimpPasta, W.fishAndChips],
  burger: [W.classicBurger, W.cheeseBurger, U.burgerFries, U.smashBurger],
  italian: [U.margherita, U.pasta, U.lasagna, U.carbonara],
  sushi: [U.sushiPlatter, U.salmonSushi, U.sushiRolls, W.ramen],
  healthy: [U.salad, U.grainBowl, U.avocadoToast, U.smoothie],
  fastfood: [U.fries, U.crispyChicken, U.wrap, U.nuggets],
  dining: [U.dining, U.restaurant, U.plated, U.caprese],
}

function hashSeed(seed: string) {
  let hash = 2166136261
  for (const char of seed) {
    hash ^= char.charCodeAt(0)
    hash = Math.imul(hash, 16777619)
  }
  return hash >>> 0
}

function hashPick<T>(items: readonly T[], seed: string) {
  return items[hashSeed(seed) % items.length]
}

function detectCuisine(text: string): Cuisine {
  const value = text.toLowerCase()
  if (
    /(egyptian|koshari|koshary|nile delta|sultan|al dar|bait el|hawawshi|ful medames|molokhia)/.test(
      value,
    )
  ) {
    return 'egyptian'
  }
  if (
    /(seafood|fish market|fisherman|poseidon|coral bay|blue wave|marina seafood|shrimp|lobster|calamari|oyster)/.test(
      value,
    )
  ) {
    return 'seafood'
  }
  if (/(sushi|sakura|nori|lotus garden|golden wok|bangkok|ramen|gyoza|asian)/.test(value)) {
    return 'sushi'
  }
  if (/(pizza|italian|milano|roma|vino|forno|pasta|lasagna|risotto)/.test(value)) {
    return 'italian'
  }
  if (
    /(healthy|vegetarian|quinoa|salad bar|sunrise|green bowl|fresh & fit|fresh and fit)/.test(value)
  ) {
    return 'healthy'
  }
  if (/(burger|diner|smokestack|flame & bun|flame and bun|big bite|urban grill)/.test(value)) {
    return 'burger'
  }
  if (/(express|quick bite|snap grill|go grill|fast food|nugget|wrap)/.test(value)) {
    return 'fastfood'
  }
  return 'dining'
}

function matchRules(text: string, rules: Rule[]) {
  for (const rule of rules) {
    if (rule.test.test(text)) return rule.image
  }
  return null
}

export function restaurantImage(name: string, hints: string[] = []) {
  const cuisine = detectCuisine([name, ...hints].join(' '))
  return hashPick(CUISINE_PHOTOS[cuisine], name)
}

export function dishImage(name: string, category = '', id = '', description = '') {
  const exact = EXACT_DISHES[normalize(name)]
  if (exact) return exact

  const haystack = `${name} ${category} ${description} ${id}`.toLowerCase()
  return (
    matchRules(haystack, DISH_RULES) ??
    matchRules(category.toLowerCase(), CATEGORY_IMAGES) ??
    U.plated
  )
}

using UnityEngine;

/// <summary>
/// Base de datos del contenido educativo con información física real sobre
/// agujeros negros, luz, y relatividad. Datos verificados científicamente.
/// </summary>
public static class SpaceEducationContent
{
    [System.Serializable]
    public class EduPanel
    {
        public string titulo;
        public string subtitulo;
        [TextArea(5, 10)] public string cuerpo;
        public string dato; // "Dato curioso" destacado

        public EduPanel(string t, string s, string c, string d)
        {
            titulo = t;
            subtitulo = s;
            cuerpo = c;
            dato = d;
        }
    }

    /// <summary>
    /// Los paneles educativos en orden de aparición durante el viaje.
    /// </summary>
    public static EduPanel[] GetPanels()
    {
        return new EduPanel[]
        {
            new EduPanel(
                "LA VELOCIDAD DE LA LUZ",
                "Eres un fotón viajando por el cosmos",
                "La luz se desplaza a 299.792.458 metros por segundo en el vacío. " +
                "Es el límite de velocidad del universo: nada con masa puede alcanzarla.\n\n" +
                "A esta velocidad, la luz podría dar 7,5 vueltas a la Tierra en un solo segundo. " +
                "Sin embargo, las distancias en el espacio son tan enormes que la luz del Sol " +
                "tarda 8 minutos y 20 segundos en llegar a nosotros.",
                "La estrella más cercana, Próxima Centauri, está a 4,24 años luz. Ves su luz de hace más de 4 años."
            ),

            new EduPanel(
                "AÑOS LUZ Y DISTANCIAS",
                "Midiendo el cosmos",
                "Un año luz es la distancia que recorre la luz en un año: unos 9,46 billones de kilómetros. " +
                "No es una medida de tiempo, sino de distancia.\n\n" +
                "Cuando observas el cielo nocturno, estás mirando al pasado. La luz de estrellas lejanas " +
                "puede haber viajado miles o millones de años antes de llegar a tus ojos.",
                "La galaxia de Andrómeda está a 2,5 millones de años luz. Su luz partió antes de que existieran los humanos."
            ),

            new EduPanel(
                "CURVATURA DEL ESPACIO-TIEMPO",
                "La relatividad general de Einstein",
                "En 1915, Einstein propuso que la gravedad no es una fuerza, sino la curvatura del " +
                "espacio-tiempo causada por la masa. Los objetos masivos deforman el tejido del universo.\n\n" +
                "La luz, aunque no tiene masa, sigue esta curvatura. Por eso su trayectoria se dobla " +
                "al pasar cerca de objetos masivos, un efecto llamado 'lente gravitacional'.",
                "En 1919, un eclipse confirmó que la luz de las estrellas se curvaba al pasar cerca del Sol."
            ),

            new EduPanel(
                "EL DISCO DE ACRECIÓN",
                "El brillo alrededor de la oscuridad",
                "La materia atraída por un agujero negro no cae directamente: forma un disco giratorio " +
                "llamado disco de acreción. La fricción calienta este material a millones de grados.\n\n" +
                "A esas temperaturas, el disco emite radiación intensa, incluyendo rayos X. " +
                "Es esta luz la que nos permite 'ver' indirectamente a los agujeros negros.",
                "El disco de acreción puede brillar más que todas las estrellas de una galaxia juntas."
            ),

            new EduPanel(
                "EL HORIZONTE DE EVENTOS",
                "El punto de no retorno",
                "El horizonte de eventos es la frontera de un agujero negro. Una vez cruzada, " +
                "nada puede escapar, ni siquiera la luz. Por eso son 'negros'.\n\n" +
                "Su radio se llama radio de Schwarzschild. Para un agujero negro con la masa del Sol, " +
                "sería de solo 3 kilómetros. Para el del centro de nuestra galaxia, unos 12 millones de km.",
                "Si el Sol se convirtiera en agujero negro, la Tierra seguiría orbitando igual: la gravedad a distancia no cambia."
            ),

            new EduPanel(
                "DILATACIÓN DEL TIEMPO",
                "El tiempo no es absoluto",
                "Cerca de un agujero negro, el tiempo transcurre más lento respecto a un observador lejano. " +
                "Este efecto, la dilatación gravitacional del tiempo, es real y medible.\n\n" +
                "Si pudieras acercarte al horizonte de eventos y regresar, habrían pasado años para tus " +
                "amigos mientras para ti solo transcurrieron minutos.",
                "El GPS corrige la dilatación del tiempo a diario: sin ello, acumularía errores de 10 km por día."
            ),

            new EduPanel(
                "LA SINGULARIDAD",
                "El corazón del misterio",
                "En el centro de un agujero negro, la teoría predice una singularidad: un punto de " +
                "densidad infinita donde las leyes de la física conocidas dejan de funcionar.\n\n" +
                "Allí, el espacio y el tiempo se comportan de formas que aún no comprendemos. " +
                "Unir la relatividad con la mecánica cuántica para explicarlo es uno de los mayores " +
                "desafíos de la física moderna.",
                "La primera imagen real de un agujero negro (M87*) se obtuvo en 2019, combinando telescopios de todo el planeta."
            )
        };
    }
}

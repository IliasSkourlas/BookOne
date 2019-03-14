using BookOne.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOne.Domain.Models
{
    public class Reaction
    {
        public Reaction()
        {
            ReceivedOn = DateTime.Now;
        }


        [Key]
        public string ActionId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ActionGiver { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser ActionReceiver { get; set; }

        [ForeignKey("BookId")]
        public virtual Book ForBook { get; set; }

        public Reactions Reaction { get; set; }

        public DateTime ReceivedOn { get; set; }
    }


    public enum Reactions
    {
        Clap,
        Shovel,
        Treat,
        PoisonousCookie,
        ConnectorBonus
    }
}
